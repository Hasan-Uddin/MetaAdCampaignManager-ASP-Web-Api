using SharedKernel;

namespace Domain.Meta.Events;

public sealed record LeadReceivedEvent(
    string LeadId,
    string FormId,
    string CampaignId) : IDomainEvent;
