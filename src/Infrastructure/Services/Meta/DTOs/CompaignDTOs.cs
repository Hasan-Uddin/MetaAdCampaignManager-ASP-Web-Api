using System.Text.Json.Serialization;

namespace Infrastructure.Services.Meta.DTOs;

internal sealed record MetaCampaign(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("objective")] string? Objective,
    [property: JsonPropertyName("configured_status")] string? ConfiguredStatus,
    [property: JsonPropertyName("buying_type")] string? BuyingType,
    [property: JsonPropertyName("budget_remaining")] string? BudgetRemaining,
    [property: JsonPropertyName("can_use_spend_cap")] bool? CanUseSpendCap,
    [property: JsonPropertyName("is_skadnetwork_attribution")] bool? IsSkadnetworkAttribution,
    [property: JsonPropertyName("created_time")]
        [property: JsonConverter(typeof(MetaDateTimeOffsetConverter))]
            DateTimeOffset CreatedTime,
    [property: JsonPropertyName("updated_time")]
        [property: JsonConverter(typeof(MetaDateTimeOffsetConverter))]
            DateTimeOffset UpdatedTime);
