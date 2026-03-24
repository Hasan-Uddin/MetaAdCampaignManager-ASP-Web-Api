
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services.Meta;
using Domain.Leads;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Meta.Leads.GetStructuredLeads;

internal sealed class GetStructuredLeadsQueryHandler(
    IApplicationDbContext context,
    IMetaApiService metaApi) : IQueryHandler<GetStructuredLeadsQuery, List<StructuredLeadResponse>>
{
    public async Task<Result<List<StructuredLeadResponse>>> Handle(GetStructuredLeadsQuery query, CancellationToken cancellationToken)
    {
        Result<List<StructuredLeadResponse>> metaResult = await metaApi.GetStructuredLeadsAsync(query.FormId, cancellationToken);

        if (metaResult.IsSuccess)
        {
            await UpsertAsync(metaResult.Value, query.FormId, cancellationToken);
            return metaResult.Value;
        }

        return await context.StructuredLeads
            .Where(l => l.FormId == query.FormId)
            .Select(l => new StructuredLeadResponse
            {
                Id = l.Id,
                FormId = l.FormId,
                AdId = l.AdId,
                CampaignId = l.CampaignId,
                AdSetId = l.AdSetId,
                CreatedAt = l.CreatedAt,
                FieldData = new LeadFieldDataResponse
                {
                    FirstName = l.FieldData.FirstName,
                    LastName = l.FieldData.LastName,
                    Email = l.FieldData.Email,
                    Country = l.FieldData.Country,
                    Phone = l.FieldData.Phone,
                    Extra = l.FieldData.Extra
                }
            })
            .ToListAsync(cancellationToken);
    }

    private async Task UpsertAsync(List<StructuredLeadResponse> items, string formId, CancellationToken ct)
    {
        var ids = items.Select(i => i.Id).ToList();
        List<StructuredLead> existing = await context.StructuredLeads.Where(l => ids.Contains(l.Id)).ToListAsync(ct);

        foreach (StructuredLeadResponse item in items)
        {
            StructuredLead? entity = existing.FirstOrDefault(l => l.Id == item.Id);
            if (entity is null)
            {
                entity = new StructuredLead { Id = item.Id, FormId = formId };
                context.StructuredLeads.Add(entity);
            }
            entity.AdId = item.AdId;
            entity.CampaignId = item.CampaignId;
            entity.AdSetId = item.AdSetId;
            entity.CreatedAt = item.CreatedAt;
            entity.SyncedAt = DateTime.UtcNow;
            entity.FieldData = new LeadFieldData
            {
                FirstName = item.FieldData.FirstName,
                LastName = item.FieldData.LastName,
                Country = item.FieldData.Country,
                Phone = item.FieldData.Phone,
                Extra = item.FieldData.Extra
            };
        }

        await context.SaveChangesAsync(ct);
    }
}
