using Domain.Leads;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Features.Meta.Webhook;

internal sealed class LeadReceivedEventHandler(ILogger<LeadReceivedEventHandler> logger)
    : IDomainEventHandler<LeadReceivedEvent>
{
    public Task Handle(LeadReceivedEvent domainEvent, CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Meta webhook: new lead received. LeadId={LeadId}, FormId={FormId}, CampaignId={CampaignId}",
                domainEvent.LeadId,
                domainEvent.FormId,
                domainEvent.CampaignId);
        }

        return Task.CompletedTask;
    }
}
