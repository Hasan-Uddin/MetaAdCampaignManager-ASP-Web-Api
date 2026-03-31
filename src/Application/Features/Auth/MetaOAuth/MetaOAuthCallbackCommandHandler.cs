using Application.Abstractions.Authentication;
using Application.Abstractions.Authentication.MetaAuth;
using Application.Abstractions.Data;
using Application.Abstractions.Interfaces;
using Application.Abstractions.Messaging;
using Application.Abstractions.WhatsApp;
using Domain.MetaSettings;
using Domain.Users;
using Domain.WhatsApp;
using Domain.WhatsAppCall;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Features.Auth.MetaOAuth;

internal sealed class MetaOAuthCallbackCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    IMetaAuthService metaAuth,
    IUserRepository userRepository,
    ITokenProvider tokenProvider,
    IWhatsAppService whatsAppService,
    IWhatsAppCallingService callingService,
    ILogger<MetaOAuthCallbackCommandHandler> logger
) : ICommandHandler<MetaOAuthCallbackCommand, MetaOAuthCallbackCommandResponse>
{
    public async Task<Result<MetaOAuthCallbackCommandResponse>> Handle(
        MetaOAuthCallbackCommand command, CancellationToken cancellationToken)
    {
        // 1. Exchange code → long-lived token
        Result<TokenResult> longLivedResult = await ExchangeTokensAsync(command, cancellationToken);
        if (longLivedResult.IsFailure)
        {
            return Result.Failure<MetaOAuthCallbackCommandResponse>(longLivedResult.Error);
        }

        // 2. Fetch Meta data
        Result<MetaPageInfo> pageInfoResult = await metaAuth.GetFirstPageAsync(
            longLivedResult.Value.AccessToken, cancellationToken);
        if (pageInfoResult.IsFailure)
        {
            return Result.Failure<MetaOAuthCallbackCommandResponse>(pageInfoResult.Error);
        }

        Result<string> adAccountResult = await metaAuth.GetFirstAdAccountIdAsync(
            longLivedResult.Value.AccessToken, cancellationToken);
        if (adAccountResult.IsFailure)
        {
            return Result.Failure<MetaOAuthCallbackCommandResponse>(adAccountResult.Error);
        }

        Result<MetaUserInfo> metaUserResult = await metaAuth.GetUserInfoAsync(
            longLivedResult.Value.AccessToken, cancellationToken);
        if (metaUserResult.IsFailure)
        {
            return Result.Failure<MetaOAuthCallbackCommandResponse>(metaUserResult.Error);
        }

        // 3. Create/update user
        User user = await UpsertUserAsync(metaUserResult.Value);

        // 4. Save Meta settings
        await UpsertMetaSettingsAsync(
            user.Id,
            longLivedResult.Value,
            pageInfoResult.Value,
            adAccountResult.Value,
            cancellationToken);

        // 5. Save WhatsApp settings — non-blocking
        WhatsAppSetting? whatsAppSetting = await TryUpsertWhatsAppSettingsAsync(
            user.Id, longLivedResult.Value, cancellationToken);

        // 6. Save call config — non-blocking
        if (whatsAppSetting is not null)
        {
            await TryUpsertCallConfigAsync(
                user.Id, whatsAppSetting, longLivedResult.Value.AccessToken, cancellationToken);
        }

        return Result.Success(new MetaOAuthCallbackCommandResponse(tokenProvider.Create(user), user.Id));
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private async Task<Result<TokenResult>> ExchangeTokensAsync(
        MetaOAuthCallbackCommand command, CancellationToken ct)
    {
        Result<TokenResult> shortLived = await metaAuth.ExchangeCodeAsync(
            command.Code, command.RedirectUri, ct);
        if (shortLived.IsFailure)
        {
            return Result.Failure<TokenResult>(shortLived.Error);
        }

        return await metaAuth.ExchangeLongLivedTokenAsync(shortLived.Value.AccessToken, ct);
    }

    private async Task<User> UpsertUserAsync(MetaUserInfo metaUser)
    {
        User? user = metaUser.Email is not null
            ? await userRepository.GetByEmailAsync(metaUser.Email)
            : await userRepository.GetByFacebookIdAsync(metaUser.Id);

        if (user is null)
        {
            user = User.CreateFromFacebook(metaUser.Id, metaUser.Name, metaUser.Email, metaUser.PictureUrl);
            await userRepository.AddAsync(user);
        }
        else
        {
            user.UpdateProfile(metaUser.Name);
        }

        await userRepository.SaveChangesAsync();
        return user;
    }

    private async Task UpsertMetaSettingsAsync(
        Guid userId,
        TokenResult token,
        MetaPageInfo pageInfo,
        string adAccountId,
        CancellationToken ct)
    {
        MetaSetting? settings = await context.MetaSettings
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

        if (settings is null)
        {
            settings = MetaSetting.Create(
                userId,
                token.AccessToken,
                pageInfo.PageId,
                adAccountId,
                pageInfo.PageAccessToken,
                dateTimeProvider.UtcNow.AddSeconds(token.ExpiresInSeconds));

            context.MetaSettings.Add(settings);
        }
        else
        {
            settings.AccessToken = token.AccessToken;
            settings.AccessTokenExpiresAt = dateTimeProvider.UtcNow.AddSeconds(token.ExpiresInSeconds);
            settings.PageId = pageInfo.PageId;
            settings.PageAccessToken = pageInfo.PageAccessToken;
            settings.AdAccountId = adAccountId;
            settings.UpdatedAt = dateTimeProvider.UtcNow;
        }

        await context.SaveChangesAsync(ct);
    }

    private async Task<WhatsAppSetting?> TryUpsertWhatsAppSettingsAsync(
        Guid userId, TokenResult token, CancellationToken ct)
    {
        Result<WhatsAppBusinessInfo> wabaResult = await whatsAppService
            .GetFirstBusinessAccountAsync(token.AccessToken, ct);

        if (wabaResult.IsFailure)
        {
            logger.LogWarning("WhatsApp Business Account not found: {Error}", wabaResult.Error.Description);
            return null;
        }

        Result<WhatsAppPhoneNumberInfo> phoneResult = await whatsAppService
            .GetFirstPhoneNumberAsync(wabaResult.Value.BusinessAccountId, token.AccessToken, ct);

        if (phoneResult.IsFailure)
        {
            logger.LogWarning("WhatsApp phone number not found: {Error}", phoneResult.Error.Description);
            return null;
        }

        WhatsAppSetting? waSetting = await context.WhatsAppSettings
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

        if (waSetting is null)
        {
            waSetting = WhatsAppSetting.Create(
                userId,
                token.AccessToken,
                wabaResult.Value.BusinessAccountId,
                phoneResult.Value.PhoneNumberId,
                phoneResult.Value.PhoneNumber,
                dateTimeProvider.UtcNow.AddSeconds(token.ExpiresInSeconds));

            context.WhatsAppSettings.Add(waSetting);
        }
        else
        {
            waSetting.AccessToken = token.AccessToken;
            waSetting.AccessTokenExpiresAt = dateTimeProvider.UtcNow.AddSeconds(token.ExpiresInSeconds);
            waSetting.BusinessAccountId = wabaResult.Value.BusinessAccountId;
            waSetting.PhoneNumberId = phoneResult.Value.PhoneNumberId;
            waSetting.PhoneNumber = phoneResult.Value.PhoneNumber;
            waSetting.UpdatedAt = dateTimeProvider.UtcNow;
        }

        await context.SaveChangesAsync(ct);
        return waSetting;
    }

    private async Task TryUpsertCallConfigAsync(
        Guid userId, WhatsAppSetting waSetting, string accessToken, CancellationToken ct)
    {
        Result<WhatsAppCallConfigSnapshot> callConfigResult = await callingService
            .GetCallSettingsAsync(waSetting.PhoneNumberId, accessToken, ct);

        if (callConfigResult.IsFailure)
        {
            logger.LogWarning("Failed to fetch WhatsApp call config: {Error}", callConfigResult.Error.Description);
            return;
        }

        WhatsAppCallConfig? callConfig = await context.WhatsAppCallConfigs
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

        if (callConfig is null)
        {
            callConfig = new WhatsAppCallConfig
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PhoneNumberId = waSetting.PhoneNumberId,
                CallingEnabled = callConfigResult.Value.CallingEnabled,
                InboundCallsEnabled = callConfigResult.Value.InboundCallsEnabled,
                CallbackRequestsEnabled = callConfigResult.Value.CallbackRequestsEnabled,
                CallHoursMode = Enum.Parse<CallHoursMode>(callConfigResult.Value.CallHoursMode, ignoreCase: true),
                UpdatedAt = dateTimeProvider.UtcNow
            };
            context.WhatsAppCallConfigs.Add(callConfig);
        }
        else
        {
            callConfig.CallingEnabled = callConfigResult.Value.CallingEnabled;
            callConfig.InboundCallsEnabled = callConfigResult.Value.InboundCallsEnabled;
            callConfig.CallbackRequestsEnabled = callConfigResult.Value.CallbackRequestsEnabled;
            callConfig.CallHoursMode = Enum.Parse<CallHoursMode>(callConfigResult.Value.CallHoursMode, ignoreCase: true);
            callConfig.UpdatedAt = dateTimeProvider.UtcNow;
        }

        await context.SaveChangesAsync(ct);
    }
}
