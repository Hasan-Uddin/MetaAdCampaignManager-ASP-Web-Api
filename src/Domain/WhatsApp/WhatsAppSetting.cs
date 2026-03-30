using Domain.Users;
using SharedKernel;

namespace Domain.WhatsApp;

public sealed class WhatsAppSetting : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string BusinessAccountId { get; set; } = string.Empty;
    public string PhoneNumberId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public User User { get; set; } = null!;

    public static WhatsAppSetting Create(
        Guid userId,
        string accessToken,
        string businessAccountId,
        string phoneNumberId,
        string phoneNumber,
        DateTime expiresAt)
    {
        return new WhatsAppSetting
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AccessToken = accessToken,
            BusinessAccountId = businessAccountId,
            PhoneNumberId = phoneNumberId,
            PhoneNumber = phoneNumber,
            AccessTokenExpiresAt = expiresAt,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
