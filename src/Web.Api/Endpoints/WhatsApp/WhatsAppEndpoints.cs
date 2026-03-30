using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Features.WhatsApp.Conversations.Create;
using Application.Features.WhatsApp.Conversations.Get;
using Application.Features.WhatsApp.Messages.Get;
using Application.Features.WhatsApp.Messages.Send;
using Application.Features.WhatsApp.Webhook;
using Domain.WhatsApp;
using Infrastructure.Services.Meta;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.WhatsApp;

internal sealed class WhatsAppEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Webhook verification
        app.MapGet("whatsapp/webhook", async (
            [FromQuery(Name = "hub.mode")] string? mode,
            [FromQuery(Name = "hub.verify_token")] string? token,
            [FromQuery(Name = "hub.challenge")] string? challenge,
            IUserContext userContext,
            IOptions<MetaApiOptions> options,
            IApplicationDbContext context,
            CancellationToken ct) =>
        {
            if (mode == "subscribe" && token == options.Value.WebhookVerifyToken)
            {
                return Results.Text(challenge);
            }

            return Results.Unauthorized();
        }).WithTags(Tags.WhatsApp);

        // Receive inbound messages + status updates
        app.MapPost("whatsapp/webhook", async (
            WhatsAppWebhookPayload payload,
            ICommandHandler<HandleWhatsAppWebhookCommand> handler,
            CancellationToken ct) =>
        {
            await handler.Handle(new HandleWhatsAppWebhookCommand(payload), ct);
            return Results.Ok();
        }).WithTags(Tags.WhatsApp);

        // Get all conversations
        app.MapGet("whatsapp/conversations", async (
            IUserContext userContext,
            IQueryHandler<GetConversationsQuery, List<ConversationResponse>> handler,
            CancellationToken ct) =>
        {
            Result<List<ConversationResponse>> result = await handler.Handle(new GetConversationsQuery(userContext.UserId), ct);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.WhatsApp).RequireAuthorization();

        // Get messages in a conversation
        app.MapGet("whatsapp/conversations/{conversationId:guid}/messages", async (
            Guid conversationId,
            IQueryHandler<GetMessagesQuery, List<MessageResponse>> handler,
            CancellationToken ct) =>
        {
            Result<List<MessageResponse>> result = await handler.Handle(new GetMessagesQuery(conversationId), ct);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.WhatsApp).RequireAuthorization();

        app.MapPost("whatsapp/conversations", async (
            CreateConversationRequest request,
            IUserContext userContext,
            ICommandHandler<CreateConversationCommand, Guid> handler,
            CancellationToken ct) =>
        {
            Result<Guid> result = await handler.Handle(
                new CreateConversationCommand(
                    userContext.UserId,
                    request.CustomerPhone,
                    request.CustomerName), ct);

            return result.Match(
                id => Results.Created($"whatsapp/conversations/{id}", id),
                CustomResults.Problem);
        }).WithTags(Tags.WhatsApp).RequireAuthorization();

        // Send a message
        app.MapPost("whatsapp/conversations/{conversationId:guid}/messages", async (
            Guid conversationId,
            SendMessageRequest request,
            IUserContext userContext,
            ICommandHandler<SendMessageCommand> handler,
            CancellationToken ct) =>
        {
            Result result = await handler.Handle(
                new SendMessageCommand(userContext.UserId, conversationId, request.Body), ct);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.WhatsApp).RequireAuthorization();
    }
}

public sealed class SendMessageRequest
{
    public string Body { get; init; } = string.Empty;
}


/*
**The complete flow:**

GET /meta/auth/login -> OAuth -> callback
  -> saves MetaSettings (as before)
  -> also fetches WABA + PhoneNumberId -> saves WhatsAppSettings
  -> WhatsApp failure is non-blocking

Inbound message:
  POST /whatsapp/webhook
    -> finds or creates Conversation by phone number
    -> saves inbound Message
    -> updates status on delivery/read receipt

Outbound message:
  POST /whatsapp/conversations/{id}/messages  { body }
    -> sends via WhatsApp API
    -> saves outbound Message
    -> updates conversation.LastMessage

GET /whatsapp/conversations        -> list all conversations
GET /whatsapp/conversations/{id}/messages -> full chat history
 */
