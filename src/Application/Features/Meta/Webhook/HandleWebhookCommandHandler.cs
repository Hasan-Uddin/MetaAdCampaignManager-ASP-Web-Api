using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Leads;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using System.Text.Json;

namespace Application.Features.Meta.Webhook;

internal sealed class HandleWebhookCommandHandler(IApplicationDbContext context)
    : ICommandHandler<HandleWebhookCommand>
{
    public async Task<Result> Handle(HandleWebhookCommand command, CancellationToken cancellationToken)
    {
        foreach (WebhookEntry entry in command.Payload.Entry)
        {
            foreach (WebhookChange change in entry.Changes)
            {
                if (change.Field == "leadgen")
                {
                    await HandleLeadgenChangeAsync(change, cancellationToken);
                }
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task HandleLeadgenChangeAsync(WebhookChange change, CancellationToken ct)
    {
        LeadgenChangeValue? value = change.Value.Deserialize<LeadgenChangeValue>();
        if (value is null)
        {
            return;
        }

        Lead? existing = await context.Leads.FirstOrDefaultAsync(l => l.Id == value.LeadgenId, ct);

        if (existing is null)
        {
            var lead = new Lead
            {
                Id = value.LeadgenId,
                FormId = value.FormId,
                AdId = value.AdId,
                CampaignId = value.CampaignId,
                AdSetId = value.AdsetId,
                CreatedAt = DateTime.UtcNow,
                SyncedAt = DateTime.UtcNow
            };
            lead.RaiseReceivedEvent();
            context.Leads.Add(lead);
        }
    }
}
