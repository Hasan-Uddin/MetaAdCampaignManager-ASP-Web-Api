namespace Application.Features.Meta.Forms;

public sealed class FormResponse
{
    public string Id { get; init; } = string.Empty;
    public string? PageId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Locale { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string PrivacyPolicyUrl { get; init; } = string.Empty;
    public string PrivacyPolicyLinkText { get; init; } = string.Empty;
    public string FollowUpActionUrl { get; init; } = string.Empty;
    public List<FormQuestionResponse> Questions { get; init; } = [];
    public DateTime CreatedAt { get; init; }
}

public sealed class FormQuestionResponse
{
    public string Id { get; init; }
    public string Key { get; init; }
    public string Type { get; init; } = string.Empty;
    public string? Label { get; init; }
}
