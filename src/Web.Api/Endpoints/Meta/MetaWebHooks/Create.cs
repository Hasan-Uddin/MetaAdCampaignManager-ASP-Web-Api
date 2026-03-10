using Application.Abstractions.Messaging;
using Application.Features.Meta.Webhook;

namespace Web.Api.Endpoints.Meta.MetaWebHooks;

internal sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
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
