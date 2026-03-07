using Application.Abstractions.Messaging;

namespace Application.Features.Meta.AdSets.Get;

public sealed record GetAdSetsQuery(string CampaignId) : IQuery<List<AdSetResponse>>;
