using Application.Abstractions.Messaging;

namespace Application.Features.WhatsApp.Messages.Send;

public sealed record SendMessageCommand(
    Guid UserId,
    Guid ConversationId,
    string Body) : ICommand;
