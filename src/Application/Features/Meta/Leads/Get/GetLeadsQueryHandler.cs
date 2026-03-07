using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Application.Abstractions.Services.Meta;
using Domain.Meta;

namespace Application.Features.Meta.Leads.Get;

internal sealed class GetLeadsQueryHandler(
    IApplicationDbContext context,
    IMetaApiService metaApi) : IQueryHandler<GetLeadsQuery, List<LeadResponse>>
{
    public async Task<Result<List<LeadResponse>>> Handle(GetLeadsQuery query, CancellationToken cancellationToken)
    {
        Result<List<LeadResponse>> metaResult = await metaApi.GetLeadsAsync(query.FormId, cancellationToken);

        if (metaResult.IsSuccess)
        {
            await UpsertAsync(metaResult.Value, query.FormId, cancellationToken);
            return metaResult.Value;
        }

        List<LeadResponse> leads = await context.Leads
            .Where(l => l.FormId == query.FormId)
            .Select(l => new LeadResponse
            {
                Id = l.Id,
                FormId = l.FormId,
                AdId = l.AdId,
                CampaignId = l.CampaignId,
                AdSetId = l.AdSetId,
                FieldData = l.FieldData,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return leads;
    }

    private async Task UpsertAsync(List<LeadResponse> items, string formId, CancellationToken ct)
    {
        var ids = items.Select(i => i.Id).ToList();
        List<Lead> existing = await context.Leads.Where(l => ids.Contains(l.Id)).ToListAsync(ct);

        foreach (LeadResponse item in items)
        {
            Lead? entity = existing.FirstOrDefault(l => l.Id == item.Id);
            if (entity is null)
            {
                entity = new Domain.Meta.Lead { Id = item.Id, FormId = formId };
                context.Leads.Add(entity);
            }
            entity.AdId = item.AdId;
            entity.CampaignId = item.CampaignId;
            entity.AdSetId = item.AdSetId;
            entity.FieldData = item.FieldData;
            entity.CreatedAt = item.CreatedAt;
            entity.SyncedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(ct);
    }
}
