using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Application.Abstractions.Authentication.MetaAuth;
using Infrastructure.Services.Meta;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Infrastructure.Services.Authentication.MetaAuth;

internal sealed class MetaAuthService(
    HttpClient httpClient,
    IOptions<MetaApiOptions> options,
    ILogger<MetaAuthService> logger) : IMetaAuthService
{
    private readonly MetaApiOptions _options = options.Value;
    private const string BaseUrl = $"https://graph.facebook.com/v25.0/";
    private const string Scopes = "pages_manage_ads,pages_read_engagement,leads_retrieval,ads_management,business_management";

    public Uri GetLoginUrl(string appId, string redirectUri, string state)
    {
        string url =
            $"https://www.facebook.com/v25.0/dialog/oauth" +
            $"?client_id={appId}" +
            $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
            $"&scope={Uri.EscapeDataString(Scopes)}" +
            $"&state={Uri.EscapeDataString(state)}" +
            $"&response_type=code";

        return new Uri(url);
    }

    public async Task<Result<TokenResult>> ExchangeCodeAsync(string code, string redirectUri, CancellationToken ct = default)
    {
        try
        {
            string url =
                $"{BaseUrl}oauth/access_token" +
                $"?client_id={_options.AppID}" +
                $"&client_secret={_options.AppSecret}" +
                $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                $"&code={code}";

            TokenResponse? response = await httpClient.GetFromJsonAsync<TokenResponse>(url, ct);

            if (response?.AccessToken is null)
            {
                return Result.Failure<TokenResult>(Error.Failure("Meta.CodeExchange", "Failed to exchange code."));
            }

            return new TokenResult(response.AccessToken, response.ExpiresIn ?? 3600);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to exchange code for token");
            return Result.Failure<TokenResult>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }

    public async Task<Result<TokenResult>> ExchangeLongLivedTokenAsync(string shortLivedToken, CancellationToken ct = default)
    {
        try
        {
            string url = $"{BaseUrl}oauth/access_token?grant_type=fb_exchange_token&client_id={_options.AppID}&client_secret={_options.AppSecret}&fb_exchange_token={shortLivedToken}";
            TokenResponse? response = await httpClient.GetFromJsonAsync<TokenResponse>(url, ct);

            if (response?.AccessToken is null)
            {
                return Result.Failure<TokenResult>(Error.Failure("Meta.TokenExchange", "Failed to exchange long-lived token."));
            }

            return new TokenResult(response.AccessToken, response.ExpiresIn ?? 5183991);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to exchange long-lived token");
            return Result.Failure<TokenResult>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }

    public async Task<Result<TokenResult>> RefreshLongLivedTokenAsync(string longLivedToken, CancellationToken ct = default)
        => await ExchangeLongLivedTokenAsync(longLivedToken, ct);

    public async Task<Result<MetaPageInfo>> GetFirstPageAsync(string longLivedToken, CancellationToken ct = default)
    {
        try
        {
            string url = $"{BaseUrl}me/accounts?fields=id,access_token&access_token={longLivedToken}";
            MetaPaginatedResponse<MetaAccount>? response = await httpClient.GetFromJsonAsync<MetaPaginatedResponse<MetaAccount>>(url, ct);
            MetaAccount? page = response?.Data.FirstOrDefault();

            if (page is null)
            {
                return Result.Failure<MetaPageInfo>(Error.Failure("Meta.NoPages", "No pages found."));
            }

            return new MetaPageInfo(page.Id, page.AccessToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch pages");
            return Result.Failure<MetaPageInfo>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }

    public async Task<Result<string>> GetFirstAdAccountIdAsync(string longLivedToken, CancellationToken ct = default)
    {
        try
        {
            string url = $"{BaseUrl}me/adaccounts?fields=id&access_token={longLivedToken}";
            MetaPaginatedResponse<MetaAdAccount>? response = await httpClient.GetFromJsonAsync<MetaPaginatedResponse<MetaAdAccount>>(url, ct);
            MetaAdAccount? account = response?.Data.FirstOrDefault();

            if (account is null)
            {
                return Result.Failure<string>(Error.Failure("Meta.NoAdAccounts", "No ad accounts found."));
            }

            return account.Id.Replace("act_", string.Empty);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch ad accounts");
            return Result.Failure<string>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }

    public async Task<Result<MetaUserInfo>> GetUserInfoAsync(string accessToken, CancellationToken ct = default)
    {
        try
        {
            string url = $"{BaseUrl}me?fields=id,name,email,picture.type(large)&access_token={accessToken}";
            MetaUserResponse? response = await httpClient.GetFromJsonAsync<MetaUserResponse>(url, ct);

            if (response is null)
            {
                return Result.Failure<MetaUserInfo>(Error.Failure("Meta.UserInfo", "Failed to fetch user info."));
            }

            return new MetaUserInfo(response.Id, response.Name, response.Email, response.Picture?.Data?.Url);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch Meta user info");
            return Result.Failure<MetaUserInfo>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }

    public async Task<Result<List<MetaAppInfo>>> GetUserAppsAsync(string accessToken, CancellationToken ct = default)
    {
        try
        {
            // get user's businesses
            string businessUrl = $"{BaseUrl}me/businesses?fields=id,name&access_token={accessToken}";
            MetaPaginatedResponse<MetaBusinessResponse>? businesses =
                await httpClient.GetFromJsonAsync<MetaPaginatedResponse<MetaBusinessResponse>>(businessUrl, ct);

            if (businesses is null || businesses.Data.Count == 0)
            {
                return Result.Failure<List<MetaAppInfo>>(
                    Error.Failure("Meta.NoBusinesses", "No Business Manager accounts found."));
            }

            var apps = new List<MetaAppInfo>();

            // get owned apps for each business
            foreach (MetaBusinessResponse business in businesses.Data)
            {
                string appsUrl = $"{BaseUrl}{business.Id}/owned_apps?fields=id,name,category&access_token={accessToken}";
                MetaPaginatedResponse<MetaAppResponse>? response =
                    await httpClient.GetFromJsonAsync<MetaPaginatedResponse<MetaAppResponse>>(appsUrl, ct);

                if (response?.Data is not null)
                {
                    apps.AddRange(response.Data.Select(a => new MetaAppInfo(a.Id, a.Name, a.Category ?? string.Empty)));
                }
            }

            return apps;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch user apps");
            return Result.Failure<List<MetaAppInfo>>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }

    // ====================== Private response models ========================

    private sealed record TokenResponse(
        [property: JsonPropertyName("access_token")] string? AccessToken,
        [property: JsonPropertyName("expires_in")] long? ExpiresIn);

    private sealed record MetaPaginatedResponse<T>(
        [property: JsonPropertyName("data")] List<T> Data);

    private sealed record MetaAccount(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("access_token")] string AccessToken);

    private sealed record MetaAdAccount(
        [property: JsonPropertyName("id")] string Id);

    private sealed record MetaUserResponse(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("email")] string? Email,
        [property: JsonPropertyName("picture")] MetaPictureWrapper? Picture);

    private sealed record MetaPictureWrapper(
        [property: JsonPropertyName("data")] MetaPictureData? Data);

    private sealed record MetaPictureData(
        [property: JsonPropertyName("url")] string? Url);

    private sealed record MetaAppResponse(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("category")] string? Category);

    private sealed record MetaBusinessResponse(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name);
}
