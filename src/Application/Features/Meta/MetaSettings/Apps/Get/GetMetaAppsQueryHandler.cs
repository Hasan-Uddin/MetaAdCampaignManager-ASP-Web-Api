using Application.Abstractions.Authentication.MetaAuth;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.MetaSettings;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Meta.MetaSettings.Apps.Get;

internal sealed class GetMetaAppsQueryHandler(
    IApplicationDbContext context,
    IMetaAuthService metaAuth) : IQueryHandler<GetMetaAppsQuery, List<MetaAppInfo>>
{
    public async Task<Result<List<MetaAppInfo>>> Handle(GetMetaAppsQuery query, CancellationToken cancellationToken)
    {
        MetaSetting? settings = await context.MetaSettings.FirstOrDefaultAsync(cancellationToken);

        if (settings is null || string.IsNullOrWhiteSpace(settings.AccessToken))
        {
            return Result.Failure<List<MetaAppInfo>>(
                Error.Failure("Meta.NotConfigured", "Login with Meta first."));
        }

        return await metaAuth.GetUserAppsAsync(settings.AccessToken, cancellationToken);
    }
}
