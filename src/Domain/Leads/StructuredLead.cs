
using SharedKernel;

namespace Domain.Leads;

public sealed class StructuredLead : Entity
{
    public string Id { get; set; } = string.Empty;
    public string FormId { get; set; } = string.Empty;
    public string AdId { get; set; } = string.Empty;
    public string CampaignId { get; set; } = string.Empty;
    public string AdSetId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime SyncedAt { get; set; }
    public LeadFieldData FieldData { get; set; } = new();

    public void RaiseReceivedEvent() =>
        Raise(new LeadReceivedEvent(Id, FormId, CampaignId));
}
