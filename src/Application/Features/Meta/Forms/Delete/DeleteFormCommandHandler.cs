using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services.Meta;
using Domain.Forms;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Meta.Forms.Delete;

internal sealed class DeleteFormCommandHandler(
    IApplicationDbContext context,
    IMetaApiService metaApi)
    : ICommandHandler<DeleteFormCommand>
{
    public async Task<Result> Handle(DeleteFormCommand command, CancellationToken cancellationToken)
    {
        Result metaResult = await metaApi.DeleteFormAsync(command.FormId, cancellationToken);

        if (metaResult.IsFailure)
        {
            return Result.Failure(metaResult.Error);
        }

        Form? form = await context.Forms.FindAsync([command.FormId], cancellationToken);

        if (form is not null)
        {
            context.Forms.Remove(form);
            await context.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
