using Application.Abstractions.Messaging;
using Application.Features.Meta.MetaSettings.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.MetaSettings;

public sealed class MetaAuthEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Manually override AppId + AppSecret (after picking from the list above)
        app.MapPut("meta/settings/app-credentials", async (
            UpdateMetaAppCredentialsCommand command,
            ICommandHandler<UpdateMetaAppCredentialsCommand> handler,
            CancellationToken ct) =>
        {
            Result result = await handler.Handle(command, ct);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.MetaSettings).RequireAuthorization();
    }
}
