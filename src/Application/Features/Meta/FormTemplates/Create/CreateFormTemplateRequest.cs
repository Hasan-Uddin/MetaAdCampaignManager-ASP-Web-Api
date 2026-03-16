
namespace Application.Features.Meta.FormTemplates.Create;

public sealed class CreateFormTemplateRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<QuestionRequest> Questions { get; init; } = [];
    public bool IsDefault { get; init; }
}
