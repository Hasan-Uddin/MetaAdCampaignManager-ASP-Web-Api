using Infrastructure.Services.Meta;
using Infrastructure.Services.Meta.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Web.Api.Endpoints.Meta.MetaWebHooks;

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Meta webhook verification (GET)
        app.MapGet("meta/webhook", async (
            [FromQuery(Name = "hub.mode")] string? mode,
            [FromQuery(Name = "hub.verify_token")] string? token,
            [FromQuery(Name = "hub.challenge")] string? challenge,
            IMetaSettingsProvider settingsProvider,
            IOptions<MetaApiOptions> options,
            CancellationToken ct) =>
        {
            if (mode == "subscribe" && token == options.Value.WebhookVerifyToken)
            {
                return Results.Text(challenge);
            }

            return Results.Unauthorized();
        })
        .WithTags(Tags.Meta);
    }
}

