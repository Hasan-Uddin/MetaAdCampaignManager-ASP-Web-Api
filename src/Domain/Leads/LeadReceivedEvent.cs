using SharedKernel;

namespace Domain.Leads;

public sealed record LeadReceivedEvent(
    string LeadId,
    string FormId,
    string CampaignId) : IDomainEvent;
