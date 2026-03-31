using Application.Abstractions.Messaging;

namespace Application.Features.WhatsApp.CallConfig.Update;

public sealed record UpdateCallConfigCommand(
    Guid UserId,
    bool CallingEnabled,
    bool InboundCallsEnabled,
    bool CallbackRequestsEnabled,
    string CallHoursMode,
    List<CallBusinessHourRequest> BusinessHours) : ICommand;

public sealed class CallBusinessHourRequest
{
    public string Day { get; init; } = string.Empty;
    public string OpenTime { get; init; } = string.Empty;
    public string CloseTime { get; init; } = string.Empty;
}
