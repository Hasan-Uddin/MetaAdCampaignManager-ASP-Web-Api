using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.MetaSettings;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Meta.MetaSettings.Update;

internal sealed class UpdateMetaAppCredentialsCommandHandler(IApplicationDbContext context)
    : ICommandHandler<UpdateMetaAppCredentialsCommand>
{
    public async Task<Result> Handle(UpdateMetaAppCredentialsCommand command, CancellationToken cancellationToken)
    {
        MetaSetting? settings = await context.MetaSettings.FirstOrDefaultAsync(cancellationToken);

        if (settings is null)
        {
            return Result.Failure(Error.Failure("Meta.NotConfigured", "Login with Meta first."));
        }

        settings.AppId = command.AppId;
        settings.AppSecret = command.AppSecret;
        settings.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
