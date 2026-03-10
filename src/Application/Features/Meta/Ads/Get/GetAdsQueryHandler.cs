using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Application.Abstractions.Services.Meta;
using Domain.Ads;

namespace Application.Features.Meta.Ads.Get;

internal sealed class GetAdsQueryHandler(
    IApplicationDbContext context,
    IMetaApiService metaApi) : IQueryHandler<GetAdsQuery, List<AdResponse>>
{
    public async Task<Result<List<AdResponse>>> Handle(GetAdsQuery query, CancellationToken cancellationToken)
    {
        Result<List<AdResponse>> metaResult = await metaApi.GetAdsAsync(query.AdSetId, cancellationToken);

        if (metaResult.IsSuccess)
        {
            await UpsertAsync(metaResult.Value, cancellationToken);
            return metaResult.Value;
        }

        List<AdResponse> ads = await context.Ads
            .Where(a => a.AdSetId == query.AdSetId)
            .Select(a => new AdResponse
            {
                Id = a.Id,
                AdSetId = a.AdSetId,
                CampaignId = a.CampaignId,
                Name = a.Name,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return ads;
    }

    private async Task UpsertAsync(List<AdResponse> items, CancellationToken ct)
    {
        var ids = items.Select(i => i.Id).ToList();
        List<Ad> existing = await context.Ads.Where(a => ids.Contains(a.Id)).ToListAsync(ct);

        foreach (AdResponse item in items)
        {
            Ad? entity = existing.FirstOrDefault(a => a.Id == item.Id);
            if (entity is null)
            {
                entity = new Ad { Id = item.Id };
                context.Ads.Add(entity);
            }
            entity.AdSetId = item.AdSetId;
            entity.CampaignId = item.CampaignId;
            entity.Name = item.Name;
            entity.Status = item.Status;
            entity.CreatedAt = item.CreatedAt;
            entity.SyncedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(ct);
    }
}
