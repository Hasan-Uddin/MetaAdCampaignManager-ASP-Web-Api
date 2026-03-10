using FluentValidation;

namespace Application.Features.Meta.Forms.Delete;

internal sealed class DeleteFormCommandValidator : AbstractValidator<DeleteFormCommand>
{
    public DeleteFormCommandValidator()
    {
        RuleFor(x => x.FormId).NotEmpty();
    }
}
