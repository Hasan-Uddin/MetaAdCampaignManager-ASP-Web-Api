using Application.Abstractions.Messaging;

namespace Application.Features.WhatsApp.Conversations.Create;

public sealed record CreateConversationCommand(
    Guid UserId,
    string CustomerPhone,
    string CustomerName) : ICommand<Guid>;
