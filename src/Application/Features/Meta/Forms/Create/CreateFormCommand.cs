using Application.Abstractions.Messaging;

namespace Application.Features.Meta.Forms.Create;

public sealed record CreateFormCommand(
    string PageId,
    string Name,
    string Locale,
    string PrivacyPolicyUrl,
    string PrivacyPolicyLinkText,
    List<QuestionRequest> Questions,
    string FollowUpActionUrl) : ICommand<string>;
