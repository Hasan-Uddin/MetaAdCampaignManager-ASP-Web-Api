using Application.Abstractions.Authentication.MetaAuth;
using Application.Abstractions.Messaging;
using Application.Features.Meta.MetaSettings.Apps.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.MetaSettings.Apps;

public sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // List all Meta apps the logged-in user administers
        app.MapGet("meta/settings/apps", async (
            IQueryHandler<GetMetaAppsQuery, List<MetaAppInfo>> handler,
            CancellationToken ct) =>
        {
            Result<List<MetaAppInfo>> result = await handler.Handle(new GetMetaAppsQuery(), ct);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.MetaSettings).RequireAuthorization();
    }
}
