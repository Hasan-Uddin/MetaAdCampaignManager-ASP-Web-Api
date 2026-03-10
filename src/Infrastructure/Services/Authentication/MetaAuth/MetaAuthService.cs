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
    private readonly string BaseUrl = options.Value.BaseUrl;

    public async Task<Result<string>> ExchangeLongLivedTokenAsync(string appId, string appSecret, string shortLivedToken, CancellationToken ct = default)
    {
        try
        {
            string url = $"{BaseUrl}oauth/access_token?grant_type=fb_exchange_token&client_id={appId}&client_secret={appSecret}&fb_exchange_token={shortLivedToken}";
            TokenResponse? response = await httpClient.GetFromJsonAsync<TokenResponse>(url, ct);

            if (response?.AccessToken is null)
            {
                return Result.Failure<string>(Error.Failure("Meta.TokenExchange", "Failed to exchange token."));
            }

            return response.AccessToken;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to exchange long-lived token");
            return Result.Failure<string>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }

    public async Task<Result<MetaPageInfo>> GetFirstPageAsync(string longLivedToken, CancellationToken ct = default)
    {
        try
        {
            string url = $"{BaseUrl}me/accounts?fields=id,access_token&access_token={longLivedToken}";
            MetaPaginatedResponse<MetaAccount>? response = await httpClient.GetFromJsonAsync<MetaPaginatedResponse<MetaAccount>>(url, ct);

            MetaAccount? page = response?.Data.FirstOrDefault();
            if (page is null)
            {
                return Result.Failure<MetaPageInfo>(Error.Failure("Meta.NoPages", "No pages found for this account."));
            }

            return new MetaPageInfo(page.Id, page.AccessToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch pages from Meta");
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

            // Meta returns "act_XXXXXXX" — strip the prefix
            return account.Id.Replace("act_", string.Empty);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch ad accounts from Meta");
            return Result.Failure<string>(Error.Failure("Meta.Unavailable", ex.Message));
        }
    }

    private sealed record TokenResponse(
        [property: JsonPropertyName("access_token")] string? AccessToken);

    private sealed record MetaPaginatedResponse<T>(
        [property: JsonPropertyName("data")] List<T> Data);

    private sealed record MetaAccount(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("access_token")] string AccessToken);

    private sealed record MetaAdAccount(
        [property: JsonPropertyName("id")] string Id);
}
