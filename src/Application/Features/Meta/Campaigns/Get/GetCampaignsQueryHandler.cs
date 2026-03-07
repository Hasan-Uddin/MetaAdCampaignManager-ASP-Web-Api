using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Application.Abstractions.Services.Meta;
using Domain.Meta;

namespace Application.Features.Meta.Campaigns.Get;

internal sealed class GetCampaignsQueryHandler(
    IApplicationDbContext context,
    IMetaApiService metaApi) : IQueryHandler<GetCampaignsQuery, List<CampaignResponse>>
{
    public async Task<Result<List<CampaignResponse>>> Handle(GetCampaignsQuery query, CancellationToken cancellationToken)
    {
        Result<List<CampaignResponse>> metaResult = await metaApi.GetCampaignsAsync(query.AdAccountId, cancellationToken);

        if (metaResult.IsSuccess)
        {
            await UpsertAsync(metaResult.Value, query.AdAccountId, cancellationToken);
            return metaResult.Value;
        }

        // Fallback to DB
        List<CampaignResponse> campaigns = await context.Campaigns
            .Where(c => c.AdAccountId == query.AdAccountId)
            .Select(c => new CampaignResponse
            {
                Id = c.Id,
                AdAccountId = c.AdAccountId,
                Name = c.Name,
                Status = c.Status,
                Objective = c.Objective,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return campaigns;
    }

    private async Task UpsertAsync(List<CampaignResponse> items, string adAccountId, CancellationToken ct)
    {
        var ids = items.Select(i => i.Id).ToList();
        List<Campaign> existing = await context.Campaigns
            .Where(c => ids.Contains(c.Id))
            .ToListAsync(ct);

        foreach (CampaignResponse item in items)
        {
            Campaign? entity = existing.FirstOrDefault(c => c.Id == item.Id);
            if (entity is null)
            {
                entity = new Campaign { Id = item.Id, AdAccountId = adAccountId };
                context.Campaigns.Add(entity);
            }
            entity.Name = item.Name;
            entity.Status = item.Status;
            entity.Objective = item.Objective;
            entity.CreatedAt = item.CreatedAt;
            entity.SyncedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(ct);
    }
}
