using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services.Meta;
using Domain.Campaigns;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Meta.Campaigns.Get;

internal sealed class GetCampaignsQueryHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
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
            .AsNoTracking()
            .Where(c => c.AdAccountId == query.AdAccountId)
            .Select(c => new CampaignResponse
            {
                Id = c.Id,
                AdAccountId = c.AdAccountId,
                Name = c.Name,
                Objective = c.Objective,
                BuyingType = c.BuyingType,
                ConfiguredStatus = c.ConfiguredStatus,
                Status = c.Status,
                BudgetRemaining = c.BudgetRemaining,
                CanUseSpendCap = c.CanUseSpendCap,
                IsSkadnetworkAttribution = c.IsSkadnetworkAttribution,
                CreatedAt = c.CreatedAt,
                SyncedAt = c.SyncedAt,
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
            entity.BuyingType = item.BuyingType;
            entity.ConfiguredStatus = item.ConfiguredStatus;
            entity.BudgetRemaining = item.BudgetRemaining;
            entity.CanUseSpendCap = item.CanUseSpendCap;
            entity.IsSkadnetworkAttribution = item.IsSkadnetworkAttribution;
            entity.CreatedAt = item.CreatedAt;
            entity.SyncedAt = dateTimeProvider.UtcNow;
        }

        await context.SaveChangesAsync(ct);
    }
}
