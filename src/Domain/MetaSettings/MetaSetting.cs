using Domain.Users;
using SharedKernel;

namespace Domain.MetaSettings;

public sealed class MetaSetting : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string PageId { get; set; } = string.Empty;
    public string AdAccountId { get; set; } = string.Empty;
    public string PageAccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public User User { get; set; }

    public static MetaSetting Create(
    Guid userId,
    string accessToken,
    string pageId,
    string adAccountId,
    string pageAccessToken,
    DateTime expiresAt)
    {
        return new MetaSetting
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AccessToken = accessToken,
            PageId = pageId,
            AdAccountId = adAccountId,
            PageAccessToken = pageAccessToken,
            AccessTokenExpiresAt = expiresAt,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
