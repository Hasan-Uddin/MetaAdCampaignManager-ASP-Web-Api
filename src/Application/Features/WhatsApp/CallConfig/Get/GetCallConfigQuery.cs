using Application.Abstractions.Messaging;

namespace Application.Features.WhatsApp.CallConfig.Get;

public sealed record GetCallConfigQuery(Guid UserId) : IQuery<CallConfigResponse>;

public sealed class CallConfigResponse
{
    public bool CallingEnabled { get; init; }
    public bool InboundCallsEnabled { get; init; }
    public bool CallbackRequestsEnabled { get; init; }
    public string CallHoursMode { get; init; } = string.Empty;
    public List<CallBusinessHourResponse> BusinessHours { get; init; } = [];
}

public sealed class CallBusinessHourResponse
{
    public string Day { get; init; } = string.Empty;
    public string OpenTime { get; init; } = string.Empty;
    public string CloseTime { get; init; } = string.Empty;
}
