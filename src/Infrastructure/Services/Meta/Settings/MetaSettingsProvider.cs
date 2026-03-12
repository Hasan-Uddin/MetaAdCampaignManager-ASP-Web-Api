using Application.Abstractions.Authentication.MetaAuth;
using Application.Abstractions.Data;
using Domain.MetaSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Infrastructure.Services.Meta.Settings;

internal sealed class MetaSettingsProvider(
    IApplicationDbContext context,
    IMetaAuthService metaAuth,
    ILogger<MetaSettingsProvider> logger) : IMetaSettingsProvider
{
    private static readonly SemaphoreSlim RefreshLock = new(1, 1);
    private static readonly TimeSpan RefreshThreshold = TimeSpan.FromDays(7);

    public async Task<Result<MetaSettingsSnapshot>> GetAsync(CancellationToken ct = default)
    {
        MetaSetting? s = await context.MetaSettings.FirstOrDefaultAsync(ct);

        if (s is null || string.IsNullOrWhiteSpace(s.AccessToken))
        {
            return Result.Failure<MetaSettingsSnapshot>(
                Error.Failure("Meta.NotConfigured", "Meta settings not configured. Call POST /meta/auth/login first."));
        }

        if (DateTime.UtcNow >= s.AccessTokenExpiresAt - RefreshThreshold)
        {
            await RefreshLock.WaitAsync(ct);
            try
            {
                await ((DbContext)context).Entry(s).ReloadAsync(ct);

                if (DateTime.UtcNow >= s.AccessTokenExpiresAt - RefreshThreshold)
                {
                    logger.LogInformation("Meta access token nearing expiry, refreshing automatically...");
                    Result<TokenResult> refreshed = await metaAuth.RefreshLongLivedTokenAsync(s.AccessToken, ct);

                    if (refreshed.IsSuccess)
                    {
                        s.AccessToken = refreshed.Value.AccessToken;
                        s.AccessTokenExpiresAt = DateTime.UtcNow.AddSeconds(refreshed.Value.ExpiresInSeconds);
                        s.UpdatedAt = DateTime.UtcNow;
                        await context.SaveChangesAsync(ct);
                        //logger.LogInformation("Meta token refreshed. New expiry: {ExpiresAt}", s.AccessTokenExpiresAt);
                    }
                    else
                    {
                        logger.LogWarning("Failed to auto-refresh Meta token: {Error}", refreshed.Error.Description);
                    }
                }
            }
            finally
            {
                RefreshLock.Release();
            }
        }

        return new MetaSettingsSnapshot(
            s.AppId,
            s.AppSecret,
            s.AccessToken,
            s.PageId,
            s.AdAccountId,
            s.PageAccessToken);
    }
}
