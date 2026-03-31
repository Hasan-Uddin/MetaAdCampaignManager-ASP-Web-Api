
namespace Application.Features.WhatsApp.CallConfig.Update;

public sealed class UpdateCallConfigRequest
{
    public bool CallingEnabled { get; init; }
    public bool InboundCallsEnabled { get; init; }
    public bool CallbackRequestsEnabled { get; init; }
    public string CallHoursMode { get; init; } = "Always";
    public List<CallBusinessHourRequest> BusinessHours { get; init; } = [];
}
