
namespace Application.Features.WhatsApp.Conversations.Create;

public sealed class CreateConversationRequest
{
    public string CustomerPhone { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
}
