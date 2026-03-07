using SharedKernel;

namespace Domain.Meta;

public sealed class Campaign : Entity
{
    public string Id { get; set; } = string.Empty;        // Meta's string ID
    public string AdAccountId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime SyncedAt { get; set; }
}
