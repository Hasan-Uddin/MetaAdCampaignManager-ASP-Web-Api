using Domain.Users;
using SharedKernel;

namespace Domain.WhatsAppCall;

public sealed class WhatsAppCallConfig : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string PhoneNumberId { get; set; } = string.Empty;

    // Master switch
    public bool CallingEnabled { get; set; }

    // Inbound call control — whether customers can call you
    public bool InboundCallsEnabled { get; set; }

    // Callback requests — offer callback when call is missed/center closed
    public bool CallbackRequestsEnabled { get; set; }

    // Business hours mode
    public CallHoursMode CallHoursMode { get; set; } = CallHoursMode.Always;

    // Business hours — stored as JSON
    public List<CallBusinessHour> BusinessHours { get; set; } = [];

    public DateTime UpdatedAt { get; set; }
    public User User { get; set; } = null!;
}

public enum CallHoursMode
{
    Always,         // calls accepted anytime
    BusinessHours,  // only during defined hours
    Disabled        // never accept calls
}

public sealed class CallBusinessHour
{
    public DayOfWeek Day { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
}
