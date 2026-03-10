using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services.Meta;
using Domain.FormQuestions;
using Domain.Forms;
using SharedKernel;

namespace Application.Features.Meta.Forms.Create;

internal sealed class CreateFormCommandHandler(
    IApplicationDbContext context,
    IMetaApiService metaApi) : ICommandHandler<CreateFormCommand, string>
{
    public async Task<Result<string>> Handle(CreateFormCommand command, CancellationToken cancellationToken)
    {
        Result<string> metaResult = await metaApi.CreateFormAsync(command, cancellationToken);
        if (metaResult.IsFailure)
        {
            return Result.Failure<string>(metaResult.Error);
        }

        string formId = metaResult.Value;

        var form = new Form
        {
            Id = formId,
            PageId = command.PageId,
            Name = command.Name,
            Locale = command.Locale,
            PrivacyPolicyUrl = command.PrivacyPolicyUrl,
            PrivacyPolicyLinkText = command.PrivacyPolicyLinkText,
            FollowUpActionUrl = command.FollowUpActionUrl,
            Questions = command.Questions
                .Select(q => new FormQuestion { 
                    Type = q.Type,
                    Label = q.Type != "CUSTOM" ? q.Label : null})
                .ToList(),
            CreatedAt = DateTime.UtcNow,
            SyncedAt = DateTime.UtcNow
        };

        context.Forms.Add(form);
        await context.SaveChangesAsync(cancellationToken);

        return formId;
    }
}
