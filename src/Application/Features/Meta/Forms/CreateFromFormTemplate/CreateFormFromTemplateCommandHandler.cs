using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services.Meta;
using Application.Features.Meta.Forms.Create;
using Domain.Forms;
using Domain.FormTemplates;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.Meta.Forms.CreateFromFormTemplate;

internal sealed class CreateFormFromTemplateCommandHandler(
    IApplicationDbContext context,
    IMetaApiService metaApi) : ICommandHandler<CreateFormFromTemplateCommand, string>
{
    public async Task<Result<string>> Handle(CreateFormFromTemplateCommand command, CancellationToken cancellationToken)
    {
        // Load template
        FormTemplate? template = await context.FormTemplates
            .FirstOrDefaultAsync(t => t.Id == command.TemplateId, cancellationToken);

        if (template is null)
        {
            return Result.Failure<string>(Error.NotFound("Template.NotFound", $"Template '{command.TemplateId}' not found."));
        }

        // Build CreateFormCommand using template questions
        var createCommand = new CreateFormCommand(
            command.PageId,
            command.Name,
            "en_US",
            command.PrivacyPolicyUrl,
            command.PrivacyPolicyLinkText,
            template.Questions
                .Select(q => new QuestionRequest { Type = q.Type, Label = q.Label })
                .ToList(),
            command.FollowUpActionUrl);

        // Create on Meta + save to DB
        Result<string> metaResult = await metaApi.CreateFormAsync(createCommand, cancellationToken);
        if (metaResult.IsFailure)
        {
            return Result.Failure<string>(metaResult.Error);
        }

        var form = new Form
        {
            Id = metaResult.Value,
            PageId = command.PageId,
            Name = command.Name,
            Locale = "en_US",
            PrivacyPolicyUrl = command.PrivacyPolicyUrl,
            PrivacyPolicyLinkText = command.PrivacyPolicyLinkText,
            FollowUpActionUrl = command.FollowUpActionUrl,
            Questions = template.Questions,
            TemplateId = command.TemplateId,   // track which template was used
            CreatedAt = DateTime.UtcNow,
            SyncedAt = DateTime.UtcNow
        };

        context.Forms.Add(form);
        await context.SaveChangesAsync(cancellationToken);

        return metaResult.Value;
    }
}
