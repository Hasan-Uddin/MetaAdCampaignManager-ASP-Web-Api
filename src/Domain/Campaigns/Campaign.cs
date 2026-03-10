using SharedKernel;

namespace Domain.Campaigns;

public sealed class Campaign : Entity
{
    public string Id { get; set; } = string.Empty;        // Meta's string ID
    public string AdAccountId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public string ConfiguredStatus { get; set; } = string.Empty;
    public string BuyingType { get; set; } = string.Empty;
    public int BudgetRemaining { get; set; }
    public bool CanUseSpendCap { get; set; }
    public bool IsSkadnetworkAttribution { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime SyncedAt { get; set; }
}
