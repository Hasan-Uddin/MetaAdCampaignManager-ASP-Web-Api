using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services.Meta;
using Domain.FormQuestions;
using Domain.Forms;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Meta.Forms.GetById;

internal sealed class GetFormByIdQueryHandler(
    IApplicationDbContext context,
    IMetaApiService metaApi) : IQueryHandler<GetFormByIdQuery, FormResponse>
{
    public async Task<Result<FormResponse>> Handle(GetFormByIdQuery query, CancellationToken cancellationToken)
    {
        Result<FormResponse> metaResult = await metaApi.GetFormByIdAsync(query.FormId, cancellationToken);

        if (metaResult.IsSuccess)
        {
            await UpsertAsync(metaResult.Value, cancellationToken);
            return metaResult.Value;
        }

        FormResponse? form = await context.Forms
            .AsNoTracking()
            .Where(f => f.Id == query.FormId)
            .Select(f => new FormResponse
            {
                Id = f.Id,
                Name = f.Name,
                Locale = f.Locale,
                Status = f.Status,
                PrivacyPolicyUrl = f.PrivacyPolicyUrl,
                PrivacyPolicyLinkText = f.PrivacyPolicyLinkText,
                FollowUpActionUrl = f.FollowUpActionUrl,
                Questions = f.Questions.Select(q => new FormQuestionResponse
                {
                    Id = q.Id ?? string.Empty,
                    Key = q.Key ?? string.Empty,
                    Type = q.Type,
                    Label = q.Label
                }).ToList(),
                CreatedAt = f.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (form is null)
        {
            return Result.Failure<FormResponse>(FormErrors.NotFound(query.FormId));
        }

        return form;
    }

    private async Task UpsertAsync(FormResponse item, CancellationToken ct)
    {
        Form? entity = await context.Forms
            .Include(f => f.Questions)
            .FirstOrDefaultAsync(f => f.Id == item.Id, ct);

        if (entity is null)
        {
            entity = new Form { Id = item.Id };
            context.Forms.Add(entity);
        }

        entity.Name = item.Name;
        entity.Locale = item.Locale;
        entity.Status = item.Status;

        entity.PrivacyPolicyUrl = item.PrivacyPolicyUrl;
        entity.PrivacyPolicyLinkText = item.PrivacyPolicyLinkText;
        entity.FollowUpActionUrl = item.FollowUpActionUrl;

        // remove old dynamic questions
        context.FormQuestions.RemoveRange(entity.Questions);

        entity.Questions = item.Questions
            .Select(q => new FormQuestion
            {
                Id = q.Id,
                Key = q.Key,
                Type = q.Type,
                Label = q.Label,
                FormId = entity.Id
            })
            .ToList();

        entity.CreatedAt = item.CreatedAt;
        entity.SyncedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
    }
}
