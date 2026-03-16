using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.FormQuestions;
using Domain.FormTemplates;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Meta.FormTemplates.Update;

internal sealed class UpdateFormTemplateCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IApplicationDbContext context)
    : ICommandHandler<UpdateFormTemplateCommand>
{
    public async Task<Result> Handle(UpdateFormTemplateCommand command, CancellationToken cancellationToken)
    {
        FormTemplate? template = await context.FormTemplates
            .FirstOrDefaultAsync(t => t.Id == command.TemplateId, cancellationToken);

        if (template is null)
        {
            return Result.Failure(TemplateErrors.NotFound(command.TemplateId));
        }

        if (command.IsDefault && !template.IsDefault)
        {
            bool defaultExists = await context.FormTemplates
                .AnyAsync(t => t.IsDefault && t.Id != command.TemplateId, cancellationToken);

            if (defaultExists)
            {
                return Result.Failure(TemplateErrors.DefaultAlreadyExists());
            }
        }

        template.Name = command.Name;
        template.Description = command.Description;
        template.Questions = command.Questions
            .Select(q => new FormQuestion { Type = q.Type, Label = q.Label })
            .ToList();
        template.IsDefault = command.IsDefault;
        template.UpdatedAt = dateTimeProvider.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
