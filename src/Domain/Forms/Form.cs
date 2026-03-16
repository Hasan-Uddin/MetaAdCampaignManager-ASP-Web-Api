using Domain.FormQuestions;
using SharedKernel;

namespace Domain.Forms;

public sealed class Form : Entity
{
    public string Id { get; set; } = string.Empty;
    public string PageId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Locale { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PrivacyPolicyUrl { get; set; } = string.Empty;
    public string PrivacyPolicyLinkText { get; set; } = string.Empty;
    public string FollowUpActionUrl { get; set; } = string.Empty;
    public List<FormQuestion> Questions { get; set; } = [];
    public Guid? TemplateId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime SyncedAt { get; set; }
}
