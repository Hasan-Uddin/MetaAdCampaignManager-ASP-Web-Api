using Domain.FormQuestions;
using SharedKernel;

namespace Domain.FormTemplates;

public sealed class FormTemplate : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<FormQuestion> Questions { get; set; } = [];
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
