using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.FormTemplates;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Meta.FormTemplates.Get;

internal sealed class GetFormTemplatesQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetFormTemplatesQuery, List<FormTemplateResponse>>
{
    public async Task<Result<List<FormTemplateResponse>>> Handle(
        GetFormTemplatesQuery query,
        CancellationToken cancellationToken)
    {
        List<FormTemplate> templates = await context.FormTemplates
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = templates.Select(t => new FormTemplateResponse
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            Questions = t.Questions.Select(q => new TemplateQuestionResponse
            {
                Type = q.Type,
                Label = q.Label
            }).ToList(),
            IsDefault = t.IsDefault,
            CreatedAt = t.CreatedAt
        }).ToList();

        return Result.Success(result);
    }
}
