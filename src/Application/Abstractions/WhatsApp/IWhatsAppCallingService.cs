using SharedKernel;

namespace Application.Abstractions.WhatsApp;

public interface IWhatsAppCallingService
{
    Task<Result<WhatsAppCallConfigSnapshot>> GetCallSettingsAsync(string phoneNumberId, string accessToken, CancellationToken ct = default);
    Task<Result> UpdateCallSettingsAsync(string phoneNumberId, string accessToken, UpdateCallSettingsRequest request, CancellationToken ct = default);
}

public sealed record WhatsAppCallConfigSnapshot(
    bool CallingEnabled,
    bool InboundCallsEnabled,
    bool CallbackRequestsEnabled,
    string CallHoursMode);

public sealed record UpdateCallSettingsRequest(
    bool CallingEnabled,
    bool InboundCallsEnabled,
    bool CallbackRequestsEnabled,
    string CallHoursMode,
    List<CallHourEntry> BusinessHours);

public sealed record CallHourEntry(string Day, string OpenTime, string CloseTime);
