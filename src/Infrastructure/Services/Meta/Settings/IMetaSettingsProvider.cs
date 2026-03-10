using Domain.MetaSettings;
using SharedKernel;

namespace Infrastructure.Services.Meta.Settings;

public interface IMetaSettingsProvider
{
    Task<Result<MetaSettingsSnapshot>> GetAsync(CancellationToken ct = default);
}

public sealed record MetaSettingsSnapshot(
    string AppId,
    string AppSecret,
    string AccessToken,
    string PageId,
    string AdAccountId,
    string WebhookVerifyToken,
    string PageAccessToken);
