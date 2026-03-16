
namespace Application.Features.Meta.Forms.CreateFromFormTemplate;

public sealed class CreateFromFormTemplateRequest
{
    public Guid TemplateId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string PrivacyPolicyUrl { get; init; } = string.Empty;
    public string PrivacyPolicyLinkText { get; init; } = string.Empty;
    public string FollowUpActionUrl { get; init; } = string.Empty;
}
