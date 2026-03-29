using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.WhatsApp;
using Domain.Conversations;
using Domain.Messages;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.WhatsApp.Messages.Send;

internal sealed class SendMessageCommandHandler(
    IApplicationDbContext context,
    IWhatsAppSettingsProvider settingsProvider,
    IWhatsAppService whatsApp) : ICommandHandler<SendMessageCommand>
{
    public async Task<Result> Handle(SendMessageCommand command, CancellationToken cancellationToken)
    {
        Result<WhatsAppSettingsSnapshot> s = await settingsProvider.GetAsync(command.UserId, cancellationToken);
        if (s.IsFailure)
        {
            return Result.Failure(s.Error);
        }

        Conversation? conversation = await context.Conversations
            .FirstOrDefaultAsync(c => c.Id == command.ConversationId, cancellationToken);

        if (conversation is null)
        {
            return Result.Failure(Error.NotFound("Conversation.NotFound", "Conversation not found."));
        }

        // Send via WhatsApp API
        Result<string> sendResult = await whatsApp.SendMessageAsync(
            s.Value.PhoneNumberId,
            conversation.CustomerPhone,
            command.Body,
            s.Value.AccessToken,
            cancellationToken);

        if (sendResult.IsFailure)
        {
            return Result.Failure(sendResult.Error);
        }

        // Save outbound message
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = command.ConversationId,
            WhatsAppMessageId = sendResult.Value,
            Body = command.Body,
            Direction = MessageDirection.Outbound,
            Status = MessageStatus.Sent,
            SentAt = DateTime.UtcNow
        };

        conversation.LastMessage = command.Body;
        conversation.LastMessageAt = DateTime.UtcNow;

        context.Messages.Add(message);
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
