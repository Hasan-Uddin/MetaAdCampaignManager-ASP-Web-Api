using Application.Abstractions.Data;
using Domain.MetaSettings;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Services.Meta.Settings;

internal sealed class MetaSettingsProvider(IApplicationDbContext context) : IMetaSettingsProvider
{
    public async Task<Result<MetaSettingsSnapshot>> GetAsync(CancellationToken ct = default)
    {
        MetaSetting? s = await context.MetaSettings.FirstOrDefaultAsync(ct);

        if (s is null || string.IsNullOrWhiteSpace(s.AccessToken))
        {
            return Result.Failure<MetaSettingsSnapshot>(
                Error.Failure("Meta.NotConfigured", "Meta settings have not been configured yet."));
        }

        return new MetaSettingsSnapshot(
            s.AppId,
            s.AppSecret,
            s.AccessToken,
            s.PageId,
            s.AdAccountId,
            s.WebhookVerifyToken,
            s.PageAccessToken);
    }
}
