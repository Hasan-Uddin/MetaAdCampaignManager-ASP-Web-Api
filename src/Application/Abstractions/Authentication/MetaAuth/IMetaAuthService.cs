using SharedKernel;

namespace Application.Abstractions.Authentication.MetaAuth;

public interface IMetaAuthService
{
    Uri GetLoginUrl(string redirectUri, string state);
    Task<Result<MetaUserInfo>> GetUserInfoAsync(string accessToken, CancellationToken ct = default);
    Task<Result<TokenResult>> ExchangeCodeAsync(string code, string redirectUri, CancellationToken ct = default);
    Task<Result<TokenResult>> ExchangeLongLivedTokenAsync(string shortLivedToken, CancellationToken ct = default);
    Task<Result<TokenResult>> RefreshLongLivedTokenAsync(string longLivedToken, CancellationToken ct = default);
    Task<Result<MetaPageInfo>> GetFirstPageAsync(string longLivedToken, CancellationToken ct = default);
    Task<Result<string>> GetFirstAdAccountIdAsync(string longLivedToken, CancellationToken ct = default);
}

public sealed record TokenResult(string AccessToken, long ExpiresInSeconds);
public sealed record MetaPageInfo(string PageId, string PageAccessToken);
public sealed record MetaUserInfo(string Id, string Name, string? Email, string? PictureUrl);
