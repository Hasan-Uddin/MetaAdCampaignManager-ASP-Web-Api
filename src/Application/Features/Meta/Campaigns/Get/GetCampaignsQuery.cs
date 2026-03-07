using Application.Abstractions.Messaging;

namespace Application.Features.Meta.Campaigns.Get;

public sealed record GetCampaignsQuery(string AdAccountId) : IQuery<List<CampaignResponse>>;
