using Application.Abstractions.Messaging;

namespace Application.Features.WhatsApp.Conversations.Get;

public sealed record GetConversationsQuery(Guid UserId) : IQuery<List<ConversationResponse>>;

public sealed class ConversationResponse
{
    public Guid Id { get; init; }
    public string CustomerPhone { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string LastMessage { get; init; } = string.Empty;
    public DateTime LastMessageAt { get; init; }
    public bool IsRead { get; init; }
}
