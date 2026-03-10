using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services.Meta;
using Application.Features.Meta.Forms;
using Domain.FormQuestions;
using Domain.Forms;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Meta.Forms.Get;

internal sealed class GetFormsQueryHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    IMetaApiService metaApi) : IQueryHandler<GetFormsQuery, List<FormResponse>>
{
    public async Task<Result<List<FormResponse>>> Handle(GetFormsQuery query, CancellationToken cancellationToken)
    {
        Result<List<FormResponse>> metaResult = await metaApi.GetFormsAsync(query.PageId, cancellationToken);

        if (metaResult.IsSuccess)
        {
            await UpsertAsync(metaResult.Value, query.PageId, cancellationToken);
            return metaResult.Value;
        }

        List<Form> forms = await context.Forms
            .AsNoTracking()
            .Where(f => f.PageId == query.PageId)
            .Include(f => f.Questions)
            .ToListAsync(cancellationToken);

        return forms.Select(f => new FormResponse
        {
            Id = f.Id,
            PageId = f.PageId,
            Name = f.Name,
            Locale = f.Locale,
            Status = f.Status,
            PrivacyPolicyUrl = f.PrivacyPolicyUrl,
            PrivacyPolicyLinkText = f.PrivacyPolicyLinkText,
            FollowUpActionUrl = f.FollowUpActionUrl,
            Questions = f.Questions?.Select(q => new FormQuestionResponse
            {
                Id = q.Id ?? string.Empty,
                Key = q.Key ?? string.Empty,
                Type = q.Type,
                Label = q.Label
            }).ToList() ?? [],
            CreatedAt = f.CreatedAt
        }).ToList();
    }

    private async Task UpsertAsync(List<FormResponse> items, string pageId, CancellationToken ct)
    {
        var ids = items.Select(i => i.Id).ToList();

        // Include Questions here so EF Core loads them
        List<Form> existing = await context.Forms
            .Include(f => f.Questions) // <-- important!
            .Where(f => ids.Contains(f.Id))
            .ToListAsync(ct);

        foreach (FormResponse item in items)
        {
            Form? entity = existing.FirstOrDefault(f => f.Id == item.Id);
            if (entity is null)
            {
                entity = new Form { Id = item.Id, PageId = pageId };
                context.Forms.Add(entity);
            }

            entity.Name = item.Name;
            entity.Locale = item.Locale;
            entity.Status = item.Status;
            entity.PrivacyPolicyUrl = item.PrivacyPolicyUrl;
            entity.PrivacyPolicyLinkText = item.PrivacyPolicyLinkText;
            entity.FollowUpActionUrl = item.FollowUpActionUrl;

            // Remove old questions before adding new ones
            if (entity.Questions.Any())
            {
                context.FormQuestions.RemoveRange(entity.Questions);
            }

            // Add new questions with FormId
            entity.Questions = item.Questions
                .Select(q => new FormQuestion
                {
                    FormId = entity.Id,
                    Type = q.Type,
                    Label = q.Label
                })
                .ToList();

            entity.CreatedAt = item.CreatedAt;
            entity.SyncedAt = dateTimeProvider.UtcNow;
        }

        await context.SaveChangesAsync(ct);
    }
}
