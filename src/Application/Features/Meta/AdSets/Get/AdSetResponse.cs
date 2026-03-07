namespace Application.Features.Meta.AdSets.Get;

public sealed class AdSetResponse
{
    public string Id { get; init; } = string.Empty;
    public string CampaignId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
