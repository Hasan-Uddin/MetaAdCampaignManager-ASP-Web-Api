
namespace Application.Features.Meta.Leads.GetStructuredLeads;

public sealed class StructuredLeadResponse
{
    public string Id { get; init; } = string.Empty;
    public string FormId { get; init; } = string.Empty;
    public string AdId { get; init; } = string.Empty;
    public string CampaignId { get; init; } = string.Empty;
    public string AdSetId { get; init; } = string.Empty;
    public LeadFieldDataResponse FieldData { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}

public sealed class LeadFieldDataResponse
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public string? Country { get; init; }
    public string? Phone { get; init; }
    public Dictionary<string, string> Extra { get; init; } = [];
}
