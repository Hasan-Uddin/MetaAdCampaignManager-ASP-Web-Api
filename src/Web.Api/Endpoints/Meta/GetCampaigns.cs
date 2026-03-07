using Application.Abstractions.Messaging;
using Application.Features.Meta.Campaigns.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Meta;

internal sealed class GetCampaigns : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meta/campaigns", async (
            string adAccountId,
            IQueryHandler<GetCampaignsQuery, List<CampaignResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<List<CampaignResponse>> result = await handler.Handle(new GetCampaignsQuery(adAccountId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Meta)
        .Produces<List<CampaignResponse>>();
    }
}
