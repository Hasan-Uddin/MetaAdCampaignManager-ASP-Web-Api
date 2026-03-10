using Domain.Forms;
using SharedKernel;

namespace Domain.FormQuestions;

public sealed class FormQuestion : Entity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Key { get; set; }
    public string FormId { get; set; } = string.Empty;
    public Form Form { get; set; } = null!;
    public string Type { get; set; } = string.Empty;
    public string? Label { get; set; }
}
