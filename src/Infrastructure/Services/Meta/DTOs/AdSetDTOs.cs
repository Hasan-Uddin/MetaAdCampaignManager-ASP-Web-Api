using System.Text.Json.Serialization;

namespace Infrastructure.Services.Meta.DTOs;

internal sealed record MetaAdSet(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("campaign_id")] string CampaignId,
    [property: JsonConverter(typeof(MetaDateTimeOffsetConverter))] DateTimeOffset CreatedTime);
