namespace Application.Features.Meta.Ads.Get;

public sealed class AdResponse
{
    public string Id { get; init; } = string.Empty;
    public string AdSetId { get; init; } = string.Empty;
    public string CampaignId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
