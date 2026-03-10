using SharedKernel;

namespace Domain.MetaSettings;

public sealed class MetaSetting : Entity
{
    public Guid Id { get; set; }
    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string PageId { get; set; } = string.Empty;
    public string AdAccountId { get; set; } = string.Empty;
    public string WebhookVerifyToken { get; set; } = string.Empty;
    public string PageAccessToken { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}
