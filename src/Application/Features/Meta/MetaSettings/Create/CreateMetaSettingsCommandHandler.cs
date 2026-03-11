//using Application.Abstractions.Authentication.MetaAuth;
//using Application.Abstractions.Data;
//using Application.Abstractions.Messaging;
//using Domain.MetaSettings;
//using Microsoft.EntityFrameworkCore;
//using SharedKernel;

//namespace Application.Features.Meta.MetaSettings.Create;


//internal sealed class CreateMetaSettingsCommandHandler(
//    IApplicationDbContext context,
//    IDateTimeProvider dateTimeProvider,
//    IMetaAuthService metaAuth) : ICommandHandler<CreateeMetaSettingsCommand>
//{
//    public async Task<Result> Handle(CreateeMetaSettingsCommand command, CancellationToken cancellationToken)
//    {
//        // Exchange short-lived token for long-lived token
//        Result<string> longLivedToken = await metaAuth.ExchangeLongLivedTokenAsync(
//            command.AppId, command.AppSecret, command.UserToken, cancellationToken);

//        if (longLivedToken.IsFailure)
//        {
//            return Result.Failure(longLivedToken.Error);
//        }

//        // Get page ID + page access token
//        Result<MetaPageInfo> pageInfo = await metaAuth.GetFirstPageAsync(longLivedToken.Value, cancellationToken);
//        if (pageInfo.IsFailure)
//        {
//            return Result.Failure(pageInfo.Error);
//        }

//        // Get ad account ID
//        Result<string> adAccountId = await metaAuth.GetFirstAdAccountIdAsync(longLivedToken.Value, cancellationToken);
//        if (adAccountId.IsFailure)
//        {
//            return Result.Failure(adAccountId.Error);
//        }

//        // Save everything to DB
//        MetaSetting? settings = await context.MetaSettings.FirstOrDefaultAsync(cancellationToken);
//        if (settings is null)
//        {
//            settings = new MetaSetting { Id = Guid.NewGuid() };
//            context.MetaSettings.Add(settings);
//        }

//        settings.AppId = command.AppId;
//        settings.AppSecret = command.AppSecret;
//        settings.AccessToken = longLivedToken.Value;
//        settings.PageId = pageInfo.Value.PageId;
//        settings.PageAccessToken = pageInfo.Value.PageAccessToken;
//        settings.AdAccountId = adAccountId.Value;
//        settings.WebhookVerifyToken = command.WebhookVerifyToken;
//        settings.UpdatedAt = dateTimeProvider.UtcNow;

//        await context.SaveChangesAsync(cancellationToken);
//        return Result.Success();
//    }
//}
