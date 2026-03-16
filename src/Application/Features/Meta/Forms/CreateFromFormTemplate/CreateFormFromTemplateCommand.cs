using Application.Abstractions.Messaging;

namespace Application.Features.Meta.Forms.CreateFromFormTemplate;

public sealed record CreateFormFromTemplateCommand(
    Guid TemplateId,
    string PageId,
    string Name,
    string PrivacyPolicyUrl,
    string PrivacyPolicyLinkText,
    string FollowUpActionUrl) : ICommand<string>;
