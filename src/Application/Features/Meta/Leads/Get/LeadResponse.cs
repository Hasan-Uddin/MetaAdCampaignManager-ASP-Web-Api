namespace Application.Features.Meta.Leads.Get;

public sealed class LeadResponse
{
    public string Id { get; init; } = string.Empty;
    public string FormId { get; init; } = string.Empty;
    public string AdId { get; init; } = string.Empty;
    public string CampaignId { get; init; } = string.Empty;
    public string AdSetId { get; init; } = string.Empty;
    public Dictionary<string, string> FieldData { get; init; } = [];
    public DateTime CreatedAt { get; init; }
}
