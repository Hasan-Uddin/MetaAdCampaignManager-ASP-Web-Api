using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Application.Abstractions.Services.Meta;
using Domain.AdSets;

namespace Application.Features.Meta.AdSets.Get;

internal sealed class GetAdSetsQueryHandler(
    IApplicationDbContext context,
    IMetaApiService metaApi) : IQueryHandler<GetAdSetsQuery, List<AdSetResponse>>
{
    public async Task<Result<List<AdSetResponse>>> Handle(GetAdSetsQuery query, CancellationToken cancellationToken)
    {
        Result<List<AdSetResponse>> metaResult = await metaApi.GetAdSetsAsync(query.CampaignId, cancellationToken);

        if (metaResult.IsSuccess)
        {
            await UpsertAsync(metaResult.Value, cancellationToken);
            return metaResult.Value;
        }

        List<AdSetResponse> adSets = await context.AdSets
            .Where(a => a.CampaignId == query.CampaignId)
            .Select(a => new AdSetResponse
            {
                Id = a.Id,
                CampaignId = a.CampaignId,
                Name = a.Name,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return adSets;
    }

    private async Task UpsertAsync(List<AdSetResponse> items, CancellationToken ct)
    {
        var ids = items.Select(i => i.Id).ToList();
        List<AdSet> existing = await context.AdSets.Where(a => ids.Contains(a.Id)).ToListAsync(ct);

        foreach (AdSetResponse item in items)
        {
            AdSet? entity = existing.FirstOrDefault(a => a.Id == item.Id);
            if (entity is null)
            {
                entity = new AdSet { Id = item.Id };
                context.AdSets.Add(entity);
            }
            entity.CampaignId = item.CampaignId;
            entity.Name = item.Name;
            entity.Status = item.Status;
            entity.CreatedAt = item.CreatedAt;
            entity.SyncedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(ct);
    }
}
