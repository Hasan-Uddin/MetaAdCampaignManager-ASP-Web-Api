
namespace Application.Features.Meta.FormTemplates;

public sealed class FormTemplateResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<TemplateQuestionResponse> Questions { get; init; } = [];
    public bool IsDefault { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed class TemplateQuestionResponse
{
    public string Type { get; init; } = string.Empty;
    public string? Label { get; init; }
}
