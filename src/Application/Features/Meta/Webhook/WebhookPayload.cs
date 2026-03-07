using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Features.Meta.Webhook;

public sealed record WebhookPayload(
    [property: JsonPropertyName("object")] string AnObject,
    [property: JsonPropertyName("entry")] List<WebhookEntry> Entry);

public sealed record WebhookEntry(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("time")] long Time,
    [property: JsonPropertyName("changes")] List<WebhookChange> Changes);

public sealed record WebhookChange(
    [property: JsonPropertyName("field")] string Field,
    [property: JsonPropertyName("value")] JsonElement Value);

// Parsed from change.value when field == "leadgen"
public sealed record LeadgenChangeValue(
    [property: JsonPropertyName("leadgen_id")] string LeadgenId,
    [property: JsonPropertyName("form_id")] string FormId,
    [property: JsonPropertyName("ad_id")] string AdId,
    [property: JsonPropertyName("campaign_id")] string CampaignId,
    [property: JsonPropertyName("adset_id")] string AdsetId,
    [property: JsonPropertyName("page_id")] string PageId);
