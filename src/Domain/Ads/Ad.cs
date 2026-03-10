using SharedKernel;

namespace Domain.Ads;

public sealed class Ad : Entity
{
    public string Id { get; set; } = string.Empty;
    public string AdSetId { get; set; } = string.Empty;
    public string CampaignId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime SyncedAt { get; set; }
}
