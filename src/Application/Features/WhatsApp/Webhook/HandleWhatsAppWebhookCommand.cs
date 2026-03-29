using System.Text.Json.Serialization;
using Application.Abstractions.Messaging;

namespace Application.Features.WhatsApp.Webhook;

public sealed record HandleWhatsAppWebhookCommand(WhatsAppWebhookPayload Payload) : ICommand;

public sealed record WhatsAppWebhookPayload(
    [property: JsonPropertyName("object")] string AnObject,
    [property: JsonPropertyName("entry")] List<WhatsAppEntry> Entry);

public sealed record WhatsAppEntry(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("changes")] List<WhatsAppChange> Changes);

public sealed record WhatsAppChange(
    [property: JsonPropertyName("value")] WhatsAppChangeValue Value,
    [property: JsonPropertyName("field")] string Field);

public sealed record WhatsAppChangeValue(
    [property: JsonPropertyName("messages")] List<WhatsAppInboundMessage>? Messages,
    [property: JsonPropertyName("contacts")] List<WhatsAppContact>? Contacts,
    [property: JsonPropertyName("statuses")] List<WhatsAppStatus>? Statuses);

public sealed record WhatsAppInboundMessage(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("from")] string From,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("timestamp")] string Timestamp,
    [property: JsonPropertyName("text")] WhatsAppText? Text);

public sealed record WhatsAppText(
    [property: JsonPropertyName("body")] string Body);

public sealed record WhatsAppContact(
    [property: JsonPropertyName("wa_id")] string WaId,
    [property: JsonPropertyName("profile")] WhatsAppProfile Profile);

public sealed record WhatsAppProfile(
    [property: JsonPropertyName("name")] string Name);

public sealed record WhatsAppStatus(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("recipient_id")] string RecipientId);
