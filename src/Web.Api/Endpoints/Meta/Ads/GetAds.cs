using Application.Abstractions.Messaging;
using Application.Features.Meta.Ads.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Meta.Ads;

internal sealed class GetAds : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meta/adsets/{adSetId}/ads", async (
            string adSetId,
            IQueryHandler<GetAdsQuery, List<AdResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<List<AdResponse>> result = await handler.Handle(new GetAdsQuery(adSetId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Ads)
        .Produces<List<AdResponse>>();
    }
}
