using Application.Abstractions.Messaging;
using Application.Features.Meta.AdSets.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Meta.AdSets;

internal sealed class GetAdSets : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meta/campaigns/{campaignId}/adsets", async (
            string campaignId,
            IQueryHandler<GetAdSetsQuery, List<AdSetResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<List<AdSetResponse>> result = await handler.Handle(new GetAdSetsQuery(campaignId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.AdSets)
        .Produces<List<AdSetResponse>>();
    }
}
