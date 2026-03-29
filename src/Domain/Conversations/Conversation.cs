using Domain.Messages;
using SharedKernel;

namespace Domain.Conversations;

public sealed class Conversation : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string LastMessage { get; set; } = string.Empty;
    public DateTime LastMessageAt { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Message> Messages { get; set; } = [];
}
