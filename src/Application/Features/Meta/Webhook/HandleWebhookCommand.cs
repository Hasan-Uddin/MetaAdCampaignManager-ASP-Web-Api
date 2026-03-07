using Application.Abstractions.Messaging;

namespace Application.Features.Meta.Webhook;

public sealed record HandleWebhookCommand(WebhookPayload Payload) : ICommand;
