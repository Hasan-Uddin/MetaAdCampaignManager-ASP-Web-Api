namespace Application.Features.Meta.Campaigns.Get;

public sealed class CampaignResponse
{
    public string Id { get; init; } = string.Empty;
    public string AdAccountId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Objective { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
