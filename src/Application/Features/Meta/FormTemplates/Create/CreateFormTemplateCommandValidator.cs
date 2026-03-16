using FluentValidation;

namespace Application.Features.Meta.FormTemplates.Create;

internal sealed class CreateFormTemplateCommandValidator : AbstractValidator<CreateFormTemplateCommand>
{
    public CreateFormTemplateCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Questions).NotEmpty();
        RuleForEach(x => x.Questions).ChildRules(q =>
        {
            q.RuleFor(x => x.Type).NotEmpty();
            q.RuleFor(x => x.Label).NotEmpty().When(x => x.Type == "CUSTOM");
        });
    }
}
