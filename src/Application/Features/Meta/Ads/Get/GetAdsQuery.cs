using Application.Abstractions.Messaging;

namespace Application.Features.Meta.Ads.Get;

public sealed record GetAdsQuery(string AdSetId) : IQuery<List<AdResponse>>;
