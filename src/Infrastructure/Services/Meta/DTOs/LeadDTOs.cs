using System.Text.Json.Serialization;

namespace Infrastructure.Services.Meta.DTOs;

internal sealed record MetaLead(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("ad_id")] string? AdId,
    [property: JsonPropertyName("campaign_id")] string? CampaignId,
    [property: JsonPropertyName("adset_id")] string? AdSetId,
    [property: JsonConverter(typeof(MetaDateTimeOffsetConverter))] DateTimeOffset CreatedTime,
    [property: JsonPropertyName("field_data")] List<MetaLeadField>? FieldData);

internal sealed record MetaLeadField(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("values")] List<string> Values);
