using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Application.Abstractions.Services.Meta;
using Application.Features.Meta.Ads.Get;
using Application.Features.Meta.AdSets.Get;
using Application.Features.Meta.Campaigns.Get;
using Application.Features.Meta.Forms;
using Application.Features.Meta.Forms.Create;
using Application.Features.Meta.Leads.Get;
using Application.Features.Meta.Leads.GetStructuredLeads;
using Infrastructure.Services.Meta.DTOs;
using Infrastructure.Services.Meta.Settings;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Infrastructure.Services.Meta;

public sealed class MetaApiService(
    HttpClient httpClient,
    //IOptions<MetaApiOptions> options,
    IMetaSettingsProvider settingsProvider,
    ILogger<MetaApiService> logger) : IMetaApiService
{

    public async Task<Result<List<CampaignResponse>>> GetCampaignsAsync(string adAccountId, CancellationToken ct = default)
    {
        Result<MetaSettingsSnapshot> s = await settingsProvider.GetAsync(ct);
        if (s.IsFailure)
        {
            return Result.Failure<List<CampaignResponse>>(s.Error);
        }

        try
        {
            string url =
                $"act_{adAccountId}/campaigns" +
                $"?fields=id,name,status,objective,created_time, updated_time," +
                $"configured_status, buying_type, budget_remaining, can_use_spend_cap, is_skadnetwork_attribution" +
                $"&access_token={s.Value.AccessToken}";

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
                ConfiguredStatus = c.ConfiguredStatus ?? string.Empty,
                BuyingType = c.BuyingType ?? string.Empty,
                BudgetRemaining = Convert.ToInt32(c.BudgetRemaining ?? "0", CultureInfo.CurrentCulture),
                CanUseSpendCap = c.CanUseSpendCap ?? false,
                IsSkadnetworkAttribution = c.IsSkadnetworkAttribution ?? false,
                CreatedAt = c.CreatedTime.UtcDateTime,
                UpdatedAt = c.UpdatedTime.UtcDateTime
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
        Result<MetaSettingsSnapshot> s = await settingsProvider.GetAsync(ct);
        if (s.IsFailure)
        {
            return Result.Failure<List<AdSetResponse>>(s.Error);
        }

        try
        {
            string url = $"{campaignId}/adsets?fields=id,name,status,campaign_id,created_time&access_token={s.Value.AccessToken}";
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
        Result<MetaSettingsSnapshot> s = await settingsProvider.GetAsync(ct);
        if (s.IsFailure)
        {
            return Result.Failure<List<AdResponse>>(s.Error);
        }

        try
        {
            string url = $"{adSetId}/ads?fields=id,name,status,adset_id,campaign_id,created_time&access_token={s.Value.AccessToken}";
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
        Result<MetaSettingsSnapshot> s = await settingsProvider.GetAsync(ct);
        if (s.IsFailure)
        {
            return Result.Failure<List<LeadResponse>>(s.Error);
        }

        try
        {
            string url = $"{formId}/leads?fields=id,ad_id,campaign_id,adset_id,created_time,field_data&access_token={s.Value.AccessToken}";
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


    public async Task<Result<List<StructuredLeadResponse>>> GetStructuredLeadsAsync(string formId, CancellationToken ct = default)
    {
        Result<MetaSettingsSnapshot> s = await settingsProvider.GetAsync(ct);
        if (s.IsFailure)
        {
            return Result.Failure<List<StructuredLeadResponse>>(s.Error);
        }

        try
        {
            string url = $"{formId}/leads?fields=id,ad_id,campaign_id,adset_id,created_time,field_data&access_token={s.Value.PageAccessToken}";
            MetaPaginatedResponse<MetaLead>? response = await httpClient.GetFromJsonAsync<MetaPaginatedResponse<MetaLead>>(url, ct);

            return response?.Data.Select(l =>
            {
                Dictionary<string, string> dict = l.FieldData?.ToDictionary(f => f.Name, f => string.Join(",", f.Values)) ?? [];

                return new StructuredLeadResponse
                {
                    Id = l.Id,
                    FormId = formId,
                    AdId = l.AdId ?? string.Empty,
                    CampaignId = l.CampaignId ?? string.Empty,
                    AdSetId = l.AdSetId ?? string.Empty,
                    CreatedAt = l.CreatedTime.UtcDateTime,
                    FieldData = new LeadFieldDataResponse
                    {
                        FirstName = dict.GetValueOrDefault("first_name"),
                        LastName = dict.GetValueOrDefault("last_name"),
                        Email = dict.GetValueOrDefault("email"),
                        Country = dict.GetValueOrDefault("country"),
                        Phone = dict.GetValueOrDefault("phone_number"),
                        Extra = dict
                            .Where(kv => kv.Key is not (
                                "first_name" or "last_name" or "email" or
                                "country" or "phone_number"))
                            .ToDictionary(kv => kv.Key, kv => kv.Value)
                    }
                };
            }).ToList() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch leads from Meta for form {FormId}", formId);
            return Result.Failure<List<StructuredLeadResponse>>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }


    public async Task<Result<List<FormResponse>>> GetFormsAsync(string pageId, CancellationToken ct = default)
    {
        Result<MetaSettingsSnapshot> s = await settingsProvider.GetAsync(ct);

        if (s.IsFailure)
        {
            return Result.Failure<List<FormResponse>>(s.Error);
        }

        MetaPaginatedResponse<MetaForm>? response = null;
        try
        {
            string url = $"{pageId}/leadgen_forms?fields=id,name,status,locale,privacy_policy,questions,follow_up_action_url,created_time&access_token={s.Value.PageAccessToken}";
            response = await httpClient.GetFromJsonAsync<MetaPaginatedResponse<MetaForm>>(url, ct);

            return response?.Data.Select(f => new FormResponse
            {
                Id = f.Id,
                PageId = pageId,
                Name = f.Name,
                Locale = f.Locale ?? "en_US",
                Status = f.Status,
                PrivacyPolicyUrl = f.PrivacyPolicy?.Url ?? string.Empty,
                PrivacyPolicyLinkText = f.PrivacyPolicy?.LinkText ?? string.Empty,
                FollowUpActionUrl = f.FollowUpActionUrl ?? string.Empty,
                Questions = f.Questions?.Select(q => new FormQuestionResponse
                {
                    Id = q.Id ?? string.Empty,
                    Key = q.Key ?? string.Empty,
                    Type = q.Type,
                    Label = q.Label
                }).ToList() ?? [],
                CreatedAt = f.CreatedTime.UtcDateTime
            }).ToList() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch forms from Meta for page {PageId}", pageId);
            logger.LogWarning("Meta response: {@Response}", response);
            return Result.Failure<List<FormResponse>>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }

    public async Task<Result<string>> CreateFormAsync(CreateFormCommand command, CancellationToken ct = default)
    {
        Result<MetaSettingsSnapshot> s = await settingsProvider.GetAsync(ct);
        if (s.IsFailure)
        {
            return Result.Failure<string>(s.Error);
        }
        try
        {
            string url = $"{command.PageId}/leadgen_forms?access_token={s.Value.PageAccessToken}";

            var body = new
            {
                name = command.Name,
                locale = command.Locale,
                privacy_policy_link_text = command.PrivacyPolicyLinkText,
                follow_up_action_url = command.FollowUpActionUrl,
                privacy_policy = new { url = command.PrivacyPolicyUrl, link_text = command.PrivacyPolicyLinkText },
                questions = command.Questions.Select(q => new
                {
                    type = q.Type,
                    label = q.Type == "CUSTOM" ? q.Label : null
                })
            };

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, body, ct);
            response.EnsureSuccessStatusCode();

            MetaCreateResponse? result = await response.Content.ReadFromJsonAsync<MetaCreateResponse>(cancellationToken: ct);
            return result?.Id ?? Result.Failure<string>(Error.Failure("Meta.Create", "No ID returned."));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to create form on Meta for page {PageId}", command.PageId);
            return Result.Failure<string>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }


    public async Task<Result<FormResponse>> GetFormByIdAsync(string formId, CancellationToken ct = default)
    {
        Result<MetaSettingsSnapshot> s = await settingsProvider.GetAsync(ct);
        if (s.IsFailure)
        {
            return Result.Failure<FormResponse>(s.Error);
        }

        try
        {
            string url = $"{formId}?fields=id,name,status,locale,privacy_policy,questions,follow_up_action_url,created_time&access_token={s.Value.PageAccessToken}";
            MetaForm? f = await httpClient.GetFromJsonAsync<MetaForm>(url, ct);
            if (f is null)
            {
                return Result.Failure<FormResponse>(Error.Failure("Meta.NotFound", "Form not found."));
            }

            return new FormResponse
            {
                Id = f.Id,
                PageId = string.Empty,   // not returned by single-form endpoint; filled from DB on upsert
                Name = f.Name,
                Locale = f.Locale ?? "en_US",
                Status = f.Status,
                PrivacyPolicyUrl = f.PrivacyPolicy?.Url ?? string.Empty,
                PrivacyPolicyLinkText = f.PrivacyPolicy?.LinkText ?? string.Empty,
                FollowUpActionUrl = f.FollowUpActionUrl ?? string.Empty,
                Questions = f.Questions?.Select(q => new FormQuestionResponse
                {
                    Id = q.Id ?? string.Empty,
                    Key = q.Key ?? string.Empty,
                    Type = q.Type,
                    Label = q.Label
                }).ToList() ?? [],
                CreatedAt = f.CreatedTime.UtcDateTime
            };
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch form {FormId} from Meta", formId);
            return Result.Failure<FormResponse>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }

    public async Task<Result> DeleteFormAsync(string formId, CancellationToken ct = default)
    {
        Result<MetaSettingsSnapshot> s = await settingsProvider.GetAsync(ct);

        if (s.IsFailure)
        {
            return Result.Failure(s.Error);
        }

        try
        {
            string url = $"{formId}?access_token={s.Value.PageAccessToken}";

            HttpResponseMessage response = await httpClient.DeleteAsync(url, ct);
            response.EnsureSuccessStatusCode();

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to delete Meta form {FormId}", formId);

            return Result.Failure(Error.Failure("Meta.Delete", ex.Message));
        }
    }

    //public async Task<Result> UpdateFormAsync(UpdateFormCommand command, CancellationToken ct = default)
    //{
    //    try
    //    {
    //        var url = $"{command.FormId}?access_token={_options.AccessToken}";
    //        var body = new
    //        {
    //            name = command.Name,
    //            locale = command.Locale,
    //            follow_up_action_url = command.FollowUpActionUrl
    //        };
    //        var response = await httpClient.PostAsJsonAsync(url, body, ct);
    //        response.EnsureSuccessStatusCode();
    //        return Result.Success();
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogWarning(ex, "Failed to update form {FormId} on Meta", command.FormId);
    //        return Result.Failure(Error.Failure("Meta.Unavailable", ex.Message));
    //    }
    //}

    // ----- Internal Meta Graph API response models -----

    private sealed record MetaPaginatedResponse<T>(
        [property: JsonPropertyName("data")] List<T> Data);
}
