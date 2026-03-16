using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.FormQuestions;
using Domain.FormTemplates;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Meta.FormTemplates.Create;

internal sealed class CreateFormTemplateCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IApplicationDbContext context)
    : ICommandHandler<CreateFormTemplateCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateFormTemplateCommand command, CancellationToken cancellationToken)
    {
        // Only one default template allowed
        if (command.IsDefault)
        {
            bool defaultExists = await context.FormTemplates
                .AnyAsync(t => t.IsDefault, cancellationToken);

            if (defaultExists)
            {
                return Result.Failure<Guid>(TemplateErrors.DefaultAlreadyExists());
            }
        }

        var template = new FormTemplate
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            Questions = command.Questions
                .Select(q => new FormQuestion { Type = q.Type, Label = q.Label })
                .ToList(),
            IsDefault = command.IsDefault,
            CreatedAt = dateTimeProvider.UtcNow,
            UpdatedAt = dateTimeProvider.UtcNow
        };

        context.FormTemplates.Add(template);
        await context.SaveChangesAsync(cancellationToken);
        return template.Id;
    }
}
