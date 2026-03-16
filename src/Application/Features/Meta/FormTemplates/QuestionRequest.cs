
namespace Application.Features.Meta.FormTemplates;

public sealed class QuestionRequest
{
    public string Type { get; init; } = string.Empty;
    public string? Label { get; init; }
}
