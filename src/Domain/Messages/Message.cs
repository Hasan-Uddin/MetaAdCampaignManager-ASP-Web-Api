using Domain.Conversations;
using SharedKernel;

namespace Domain.Messages;

public sealed class Message : Entity
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string WhatsAppMessageId { get; set; } = string.Empty;  // Meta's message ID
    public string Body { get; set; } = string.Empty;
    public MessageDirection Direction { get; set; }
    public MessageStatus Status { get; set; }
    public DateTime SentAt { get; set; }
    public Conversation Conversation { get; set; } = null!;
}

public enum MessageDirection { Inbound, Outbound }
public enum MessageStatus { Sent, Delivered, Read, Failed }
