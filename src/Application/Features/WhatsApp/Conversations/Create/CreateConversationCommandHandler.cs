using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Conversations;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.WhatsApp.Conversations.Create;

internal sealed class CreateConversationCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CreateConversationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateConversationCommand command, CancellationToken cancellationToken)
    {
        // Prevent duplicate conversation for same phone number
        bool exists = await context.Conversations
            .AnyAsync(c => c.UserId == command.UserId
                        && c.CustomerPhone == command.CustomerPhone, cancellationToken);

        if (exists)
        {
            return Result.Failure<Guid>(
                Error.Conflict("Conversation.AlreadyExists",
                    $"A conversation with {command.CustomerPhone} already exists."));
        }

        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            CustomerPhone = command.CustomerPhone,
            CustomerName = command.CustomerName,
            LastMessage = string.Empty,
            LastMessageAt = DateTime.UtcNow,
            IsRead = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Conversations.Add(conversation);
        await context.SaveChangesAsync(cancellationToken);

        return conversation.Id;
    }
}
