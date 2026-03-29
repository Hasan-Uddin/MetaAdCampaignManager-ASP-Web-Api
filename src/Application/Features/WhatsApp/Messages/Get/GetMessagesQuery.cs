using Application.Abstractions.Messaging;

namespace Application.Features.WhatsApp.Messages.Get;

public sealed record GetMessagesQuery(Guid ConversationId) : IQuery<List<MessageResponse>>;

public sealed class MessageResponse
{
    public Guid Id { get; init; }
    public string Body { get; init; } = string.Empty;
    public string Direction { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime SentAt { get; init; }
}
