
using SharedKernel;

namespace Domain.FormTemplates;

public static class TemplateErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Template.NotFound", $"Template '{id}' was not found.");

    public static Error DefaultAlreadyExists() =>
        Error.Conflict("Template.DefaultExists", "A default template already exists.");
}
