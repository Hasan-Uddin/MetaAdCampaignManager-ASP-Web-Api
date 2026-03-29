using Application.Abstractions.Authentication;
using Application.Abstractions.Authentication.MetaAuth;
using Application.Abstractions.Data;
using Application.Abstractions.Interfaces;
using Application.Abstractions.Messaging;
using Application.Abstractions.WhatsApp;
using Domain.MetaSettings;
using Domain.Users;
using Domain.WhatsApp;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Auth.MetaOAuth;

internal sealed class MetaOAuthCallbackCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    IMetaAuthService metaAuth,
    IUserRepository userRepository,
    ITokenProvider tokenProvider,
    IWhatsAppService whatsAppService
) : ICommandHandler<MetaOAuthCallbackCommand, MetaOAuthCallbackCommandResponse>
{
    public async Task<Result<MetaOAuthCallbackCommandResponse>> Handle(MetaOAuthCallbackCommand command, CancellationToken cancellationToken)
    {
        // Exchange code → long-lived access token
        Result<TokenResult> tokenResult = await metaAuth.ExchangeCodeAsync(
            command.Code, command.RedirectUri, cancellationToken);
        if (tokenResult.IsFailure)
        {
            return Result.Failure<MetaOAuthCallbackCommandResponse>(tokenResult.Error);
        }

        Result<TokenResult> longLivedResult = await metaAuth.ExchangeLongLivedTokenAsync(tokenResult.Value.AccessToken, cancellationToken);
        if (longLivedResult.IsFailure)
        {
            return Result.Failure<MetaOAuthCallbackCommandResponse>(longLivedResult.Error);
        }

        // Fetch page info + ad account
        Result<MetaPageInfo> pageInfoResult = await metaAuth.GetFirstPageAsync(longLivedResult.Value.AccessToken, cancellationToken);
        if (pageInfoResult.IsFailure)
        {
            return Result.Failure<MetaOAuthCallbackCommandResponse>(pageInfoResult.Error);
        }

        Result<string> adAccountResult = await metaAuth.GetFirstAdAccountIdAsync(longLivedResult.Value.AccessToken, cancellationToken);
        if (adAccountResult.IsFailure)
        {
            return Result.Failure<MetaOAuthCallbackCommandResponse>(adAccountResult.Error);
        }

        // Fetch Meta user info
        Result<MetaUserInfo> metaUserResult = await metaAuth.GetUserInfoAsync(longLivedResult.Value.AccessToken, cancellationToken);
        if (metaUserResult.IsFailure)
        {
            return Result.Failure<MetaOAuthCallbackCommandResponse>(metaUserResult.Error);
        }

        MetaUserInfo metaUser = metaUserResult.Value;

        // Create/update User
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

        // Create/update singleton MetaSettings row
        MetaSetting? settings = await context.MetaSettings
            .FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

        if (settings is null)
        {
            settings = MetaSetting.Create(
                user.Id,
                longLivedResult.Value.AccessToken,
                pageInfoResult.Value.PageId,
                adAccountResult.Value,
                pageInfoResult.Value.PageAccessToken,
                dateTimeProvider.UtcNow.AddSeconds(longLivedResult.Value.ExpiresInSeconds)
            );

            context.MetaSettings.Add(settings);
        }
        else
        {
            settings.AccessToken = longLivedResult.Value.AccessToken;
            settings.AccessTokenExpiresAt = dateTimeProvider.UtcNow.AddSeconds(longLivedResult.Value.ExpiresInSeconds);
            settings.PageId = pageInfoResult.Value.PageId;
            settings.PageAccessToken = pageInfoResult.Value.PageAccessToken;
            settings.AdAccountId = adAccountResult.Value;
            settings.UpdatedAt = dateTimeProvider.UtcNow;
        }

        await context.SaveChangesAsync(cancellationToken);

        // Fetch WhatsApp Business Account
        Result<WhatsAppBusinessInfo> wabaResult = await whatsAppService
            .GetFirstBusinessAccountAsync(longLivedResult.Value.AccessToken, cancellationToken);

        if (wabaResult.IsSuccess)
        {
            Result<WhatsAppPhoneNumberInfo> phoneResult = await whatsAppService
                .GetFirstPhoneNumberAsync(wabaResult.Value.BusinessAccountId, longLivedResult.Value.AccessToken, cancellationToken);

            if (phoneResult.IsSuccess)
            {
                WhatsAppSetting? waSetting = await context.WhatsAppSettings
                    .FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

                if (waSetting is null)
                {
                    waSetting = WhatsAppSetting.Create(
                        user.Id,
                        longLivedResult.Value.AccessToken,
                        wabaResult.Value.BusinessAccountId,
                        phoneResult.Value.PhoneNumberId,
                        phoneResult.Value.PhoneNumber,
                        Guid.NewGuid().ToString(),   // webhook verify token auto-generated
                        dateTimeProvider.UtcNow.AddSeconds(longLivedResult.Value.ExpiresInSeconds));

                    context.WhatsAppSettings.Add(waSetting);
                }
                else
                {
                    waSetting.AccessToken = longLivedResult.Value.AccessToken;
                    waSetting.AccessTokenExpiresAt = dateTimeProvider.UtcNow.AddSeconds(longLivedResult.Value.ExpiresInSeconds);
                    waSetting.BusinessAccountId = wabaResult.Value.BusinessAccountId;
                    waSetting.PhoneNumberId = phoneResult.Value.PhoneNumberId;
                    waSetting.PhoneNumber = phoneResult.Value.PhoneNumber;
                    waSetting.UpdatedAt = dateTimeProvider.UtcNow;
                }

                await context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                //logger.LogWarning("WhatsApp phone number not found: {Error}", phoneResult.Error.Description);
            }
        }
        else
        {
            //logger.LogWarning("WhatsApp Business Account not found: {Error}", wabaResult.Error.Description);
        }

        return Result.Success(new MetaOAuthCallbackCommandResponse(tokenProvider.Create(user), user.Id));
    }
}
