using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.FormTemplates;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Meta.FormTemplates.Delete;

internal sealed class DeleteFormTemplateCommandHandler(IApplicationDbContext context)
    : ICommandHandler<DeleteFormTemplateCommand>
{
    public async Task<Result> Handle(DeleteFormTemplateCommand command, CancellationToken cancellationToken)
    {
        FormTemplate? template = await context.FormTemplates
            .FirstOrDefaultAsync(t => t.Id == command.TemplateId, cancellationToken);

        if (template is null)
        {
            return Result.Failure(TemplateErrors.NotFound(command.TemplateId));
        }

        context.FormTemplates.Remove(template);
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
