using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.FormTemplates;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Meta.FormTemplates.GetById;

internal class GetFormTemplateByIdQueryHandler
{
}

internal sealed class GetTemplateByIdQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetFormTemplateByIdQuery, FormTemplateResponse>
{
    public async Task<Result<FormTemplateResponse>> Handle(GetFormTemplateByIdQuery query, CancellationToken cancellationToken)
    {
        FormTemplateResponse? template = await context.FormTemplates
            .Where(t => t.Id == query.TemplateId)
            .Select(t => new FormTemplateResponse
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Questions = t.Questions
                    .Select(q => new TemplateQuestionResponse { Type = q.Type, Label = q.Label })
                    .ToList(),
                IsDefault = t.IsDefault,
                CreatedAt = t.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (template is null)
        {
            return Result.Failure<FormTemplateResponse>(TemplateErrors.NotFound(query.TemplateId));
        }

        return template;
    }
}
