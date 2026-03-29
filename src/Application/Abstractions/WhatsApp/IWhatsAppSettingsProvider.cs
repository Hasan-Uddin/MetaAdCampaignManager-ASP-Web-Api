using SharedKernel;

namespace Application.Abstractions.WhatsApp;

public interface IWhatsAppSettingsProvider
{
    Task<Result<WhatsAppSettingsSnapshot>> GetAsync(Guid userId, CancellationToken ct = default);
}

public sealed record WhatsAppSettingsSnapshot(
    string AccessToken,
    string BusinessAccountId,
    string PhoneNumberId,
    string PhoneNumber);
