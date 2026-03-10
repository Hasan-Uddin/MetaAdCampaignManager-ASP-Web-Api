using SharedKernel;

namespace Domain.Forms;

public static class FormErrors
{
    public static Error NotFound(string id) =>
        Error.NotFound("Form.NotFound", $"Form '{id}' was not found.");
}
