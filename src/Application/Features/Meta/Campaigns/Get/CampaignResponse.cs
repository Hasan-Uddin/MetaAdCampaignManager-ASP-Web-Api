namespace Application.Features.Meta.Campaigns.Get;

public sealed class CampaignResponse
{
    public string Id { get; init; } = string.Empty;
    public string AdAccountId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Objective { get; init; } = string.Empty;
    public string ConfiguredStatus { get; set; } = string.Empty;
    public string BuyingType { get; set; } = string.Empty;
    public int BudgetRemaining { get; set; }
    public bool CanUseSpendCap { get; set; }
    public bool IsSkadnetworkAttribution { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime SyncedAt { get; set; }
}
