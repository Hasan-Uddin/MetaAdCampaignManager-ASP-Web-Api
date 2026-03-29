using SharedKernel;

namespace Application.Abstractions.WhatsApp;

public interface IWhatsAppService
{
    Task<Result<string>> SendMessageAsync(string phoneNumberId, string toPhone, string body, string accessToken, CancellationToken ct = default);
    Task<Result<WhatsAppBusinessInfo>> GetFirstBusinessAccountAsync(string accessToken, CancellationToken ct = default);
    Task<Result<WhatsAppPhoneNumberInfo>> GetFirstPhoneNumberAsync(string wabaId, string accessToken, CancellationToken ct = default);
}

public sealed record WhatsAppBusinessInfo(string BusinessAccountId);
public sealed record WhatsAppPhoneNumberInfo(string PhoneNumberId, string PhoneNumber);
