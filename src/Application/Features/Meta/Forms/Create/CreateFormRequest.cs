namespace Application.Features.Meta.Forms.Create;

public sealed class CreateFormRequest
{
    public string Name { get; init; } = string.Empty;
    public string Locale { get; init; } = "en_US";
    public string PrivacyPolicyLinkText { get; init; }
    public string FollowUpActionUrl { get; init; }
    public PrivacyPolicyRequest PrivacyPolicy { get; init; } = new();
    public List<QuestionRequest> Questions { get; init; } = [];
}

public sealed class PrivacyPolicyRequest
{
    public string Url { get; init; } = string.Empty;
    public string LinkText { get; init; } = string.Empty;
}

public sealed class QuestionRequest
{
    public string Type { get; init; } = string.Empty;
    public string? Label { get; init; }
}
