using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Conversations;
using Domain.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Features.WhatsApp.Webhook;

internal sealed class HandleWhatsAppWebhookCommandHandler(
    IApplicationDbContext context,
    ILogger<HandleWhatsAppWebhookCommandHandler> logger) : ICommandHandler<HandleWhatsAppWebhookCommand>
{
    public async Task<Result> Handle(HandleWhatsAppWebhookCommand command, CancellationToken cancellationToken)
    {
        foreach (WhatsAppEntry entry in command.Payload.Entry)
        {
            foreach (WhatsAppChange? change in entry.Changes.Where(c => c.Field == "messages"))
            {
                await HandleMessagesAsync(change.Value, cancellationToken);
                await HandleStatusUpdatesAsync(change.Value, cancellationToken);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task HandleMessagesAsync(WhatsAppChangeValue value, CancellationToken ct)
    {
        if (value.Messages is null)
        {
            return;
        }

        foreach (WhatsAppInboundMessage? inbound in value.Messages.Where(m => m.Type == "text"))
        {
            string customerPhone = inbound.From;
            string customerName = value.Contacts?
                .FirstOrDefault(c => c.WaId == customerPhone)?.Profile.Name ?? customerPhone;

            // Find or create conversation
            Conversation? conversation = await context.Conversations
                .FirstOrDefaultAsync(c => c.CustomerPhone == customerPhone, ct);

            if (conversation is null)
            {
                conversation = new Conversation
                {
                    Id = Guid.NewGuid(),
                    CustomerPhone = customerPhone,
                    CustomerName = customerName,
                    CreatedAt = DateTime.UtcNow,
                    LastMessageAt = DateTime.UtcNow
                };
                context.Conversations.Add(conversation);
            }

            string body = inbound.Text?.Body ?? string.Empty;
            conversation.LastMessage = body;
            conversation.LastMessageAt = DateTime.UtcNow;
            conversation.IsRead = false;

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                WhatsAppMessageId = inbound.Id,
                Body = body,
                Direction = MessageDirection.Inbound,
                Status = MessageStatus.Delivered,
                SentAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(
                    inbound.Timestamp, 
                    System.Globalization.NumberStyles.Integer,
                    System.Globalization.CultureInfo.InvariantCulture)).UtcDateTime
            };

            context.Messages.Add(message);
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "WhatsApp inbound message from {Phone}: {Body}", customerPhone, body);
            }
        }
    }

    private async Task HandleStatusUpdatesAsync(WhatsAppChangeValue value, CancellationToken ct)
    {
        if (value.Statuses is null)
        {
            return;
        }

        foreach (WhatsAppStatus status in value.Statuses)
        {
            Message? message = await context.Messages
                .FirstOrDefaultAsync(m => m.WhatsAppMessageId == status.Id, ct);

            if (message is null)
            {
                continue;
            }

            message.Status = status.Status switch
            {
                "delivered" => MessageStatus.Delivered,
                "read" => MessageStatus.Read,
                "failed" => MessageStatus.Failed,
                _ => message.Status
            };

            if(logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "WhatsApp message {Id} status updated to {Status}", status.Id, status.Status);
            }
        }
    }
}
