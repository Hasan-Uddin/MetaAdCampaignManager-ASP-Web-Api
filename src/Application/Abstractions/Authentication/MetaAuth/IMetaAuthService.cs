using SharedKernel;

namespace Application.Abstractions.Authentication.MetaAuth;

public interface IMetaAuthService
{
    Task<Result<string>> ExchangeLongLivedTokenAsync(string appId, string appSecret, string shortLivedToken, CancellationToken ct = default);
    Task<Result<MetaPageInfo>> GetFirstPageAsync(string longLivedToken, CancellationToken ct = default);
    Task<Result<string>> GetFirstAdAccountIdAsync(string longLivedToken, CancellationToken ct = default);
}

public sealed record MetaPageInfo(string PageId, string PageAccessToken);
