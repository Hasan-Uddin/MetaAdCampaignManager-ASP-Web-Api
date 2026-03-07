using Application.Abstractions.Services.Meta;
using Application.Features.Meta.Ads.Get;
using Application.Features.Meta.AdSets.Get;
using Application.Features.Meta.Campaigns.Get;
using Application.Features.Meta.Leads.Get;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services.Meta;

public sealed class MetaApiService(
    HttpClient httpClient,
    IOptions<MetaApiOptions> options,
    ILogger<MetaApiService> logger) : IMetaApiService
{
    private readonly MetaApiOptions _options = options.Value;

    public async Task<Result<List<CampaignResponse>>> GetCampaignsAsync(string adAccountId, CancellationToken ct = default)
    {
        try
        {
            string url =
                $"act_{adAccountId}/campaigns" +
                $"?fields=id,name,status,objective,created_time" +
                $"&access_token={_options.AccessToken}";

            logger.LogWarning("Meta API Request URL: {Url}", url);

            HttpResponseMessage response = await httpClient.GetAsync(url, ct);

            string body = await response.Content.ReadAsStringAsync(ct);

            logger.LogWarning("Meta API Response: {Body}", body);

            response.EnsureSuccessStatusCode();

            MetaPaginatedResponse<MetaCampaign>? result =
                System.Text.Json.JsonSerializer.Deserialize<MetaPaginatedResponse<MetaCampaign>>(body);

            return result?.Data.Select(c => new CampaignResponse
            {
                Id = c.Id,
                AdAccountId = adAccountId,
                Name = c.Name,
                Status = c.Status,
                Objective = c.Objective ?? string.Empty,
                CreatedAt = c.CreatedTime.UtcDateTime
            }).ToList() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch campaigns from Meta for account {AdAccountId}", adAccountId);
            return Result.Failure<List<CampaignResponse>>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }

    public async Task<Result<List<AdSetResponse>>> GetAdSetsAsync(string campaignId, CancellationToken ct = default)
    {
        try
        {
            string url = $"{campaignId}/adsets?fields=id,name,status,campaign_id,created_time&access_token={_options.AccessToken}";
            MetaPaginatedResponse<MetaAdSet>? response = await httpClient.GetFromJsonAsync<MetaPaginatedResponse<MetaAdSet>>(url, ct);

            return response?.Data.Select(a => new AdSetResponse
            {
                Id = a.Id,
                CampaignId = a.CampaignId,
                Name = a.Name,
                Status = a.Status,
                CreatedAt = a.CreatedTime.UtcDateTime
            }).ToList() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch ad sets from Meta for campaign {CampaignId}", campaignId);
            return Result.Failure<List<AdSetResponse>>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }

    public async Task<Result<List<AdResponse>>> GetAdsAsync(string adSetId, CancellationToken ct = default)
    {
        try
        {
            string url = $"{adSetId}/ads?fields=id,name,status,adset_id,campaign_id,created_time&access_token={_options.AccessToken}";
            MetaPaginatedResponse<MetaAd>? response = await httpClient.GetFromJsonAsync<MetaPaginatedResponse<MetaAd>>(url, ct);

            return response?.Data.Select(a => new AdResponse
            {
                Id = a.Id,
                AdSetId = a.AdSetId,
                CampaignId = a.CampaignId,
                Name = a.Name,
                Status = a.Status,
                CreatedAt = a.CreatedTime.UtcDateTime
            }).ToList() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch ads from Meta for ad set {AdSetId}", adSetId);
            return Result.Failure<List<AdResponse>>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }

    public async Task<Result<List<LeadResponse>>> GetLeadsAsync(string formId, CancellationToken ct = default)
    {
        try
        {
            string url = $"{formId}/leads?fields=id,ad_id,campaign_id,adset_id,created_time,field_data&access_token={_options.AccessToken}";
            MetaPaginatedResponse<MetaLead>? response = await httpClient.GetFromJsonAsync<MetaPaginatedResponse<MetaLead>>(url, ct);

            return response?.Data.Select(l => new LeadResponse
            {
                Id = l.Id,
                FormId = formId,
                AdId = l.AdId ?? string.Empty,
                CampaignId = l.CampaignId ?? string.Empty,
                AdSetId = l.AdSetId ?? string.Empty,
                FieldData = l.FieldData?.ToDictionary(f => f.Name, f => string.Join(",", f.Values)) ?? [],
                CreatedAt = l.CreatedTime.UtcDateTime
            }).ToList() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch leads from Meta for form {FormId}", formId);
            return Result.Failure<List<LeadResponse>>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }

    // ----- Internal Meta Graph API response models -----

    private sealed record MetaPaginatedResponse<T>(
        [property: JsonPropertyName("data")] List<T> Data);

    private sealed record MetaCampaign(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("objective")] string? Objective,
        [property: JsonConverter(typeof(MetaDateTimeOffsetConverter))] DateTimeOffset CreatedTime);

    private sealed record MetaAdSet(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("campaign_id")] string CampaignId,
        [property: JsonConverter(typeof(MetaDateTimeOffsetConverter))] DateTimeOffset CreatedTime);

    private sealed record MetaAd(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("adset_id")] string AdSetId,
        [property: JsonPropertyName("campaign_id")] string CampaignId,
        [property: JsonConverter(typeof(MetaDateTimeOffsetConverter))] DateTimeOffset CreatedTime);

    private sealed record MetaLead(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("ad_id")] string? AdId,
        [property: JsonPropertyName("campaign_id")] string? CampaignId,
        [property: JsonPropertyName("adset_id")] string? AdSetId,
        [property: JsonConverter(typeof(MetaDateTimeOffsetConverter))] DateTimeOffset CreatedTime,
        [property: JsonPropertyName("field_data")] List<MetaLeadField>? FieldData);

    private sealed record MetaLeadField(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("values")] List<string> Values);
}
