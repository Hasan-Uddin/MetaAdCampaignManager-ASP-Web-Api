using System.Text.Json.Serialization;

namespace Infrastructure.Services.Meta.DTOs;

internal sealed record MetaForm(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("locale")] string? Locale,
    [property: JsonPropertyName("privacy_policy")] MetaPrivacyPolicy? PrivacyPolicy,
    [property: JsonPropertyName("questions")] List<MetaFormQuestion>? Questions,
    [property: JsonPropertyName("follow_up_action_url")] string? FollowUpActionUrl,
    [property: JsonPropertyName("created_time")]
    [property: JsonConverter(typeof(MetaDateTimeOffsetConverter))]
        DateTimeOffset CreatedTime);

internal sealed record MetaPrivacyPolicy(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("link_text")] string LinkText);

internal sealed record MetaFormQuestion(
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("key")] string? Key,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("label")] string? Label);

internal sealed record MetaCreateResponse(
    [property: JsonPropertyName("id")] string Id);
