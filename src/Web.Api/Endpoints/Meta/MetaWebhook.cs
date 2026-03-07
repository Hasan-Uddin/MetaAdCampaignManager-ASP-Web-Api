using Application.Abstractions.Messaging;
using Application.Features.Meta.Webhook;
using Infrastructure.Services.Meta;
using Microsoft.Extensions.Options;

namespace Web.Api.Endpoints.Meta;

internal sealed class MetaWebhook : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Meta webhook verification (GET)
        app.MapGet("meta/webhook", (
            [Microsoft.AspNetCore.Mvc.FromQuery(Name = "hub.mode")] string? mode,
            [Microsoft.AspNetCore.Mvc.FromQuery(Name = "hub.verify_token")] string? token,
            [Microsoft.AspNetCore.Mvc.FromQuery(Name = "hub.challenge")] string? challenge,
            IOptions<MetaApiOptions> options) =>
        {
            if (mode == "subscribe" && token == options.Value.WebhookVerifyToken)
            {
                return Results.Ok(challenge);
            }

            return Results.Unauthorized();
        })
        .WithTags(Tags.Meta);

        // Meta webhook events (POST)
        app.MapPost("meta/webhook", async (
            WebhookPayload payload,
            ICommandHandler<HandleWebhookCommand> handler,
            CancellationToken cancellationToken) =>
        {
            await handler.Handle(new HandleWebhookCommand(payload), cancellationToken);
            return Results.Ok();
        })
        .WithTags(Tags.Meta);
    }
}
