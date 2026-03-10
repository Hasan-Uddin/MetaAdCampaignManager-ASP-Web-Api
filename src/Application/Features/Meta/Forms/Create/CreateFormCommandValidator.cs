using FluentValidation;

namespace Application.Features.Meta.Forms.Create;

internal sealed class CreateFormCommandValidator : AbstractValidator<CreateFormCommand>
{
    public CreateFormCommandValidator()
    {
        RuleFor(x => x.PageId).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Locale).NotEmpty().MaximumLength(20);
        RuleFor(x => x.PrivacyPolicyUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.PrivacyPolicyLinkText).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Questions).NotEmpty();
        RuleForEach(x => x.Questions).ChildRules(q =>
        {
            q.RuleFor(x => x.Type).NotEmpty();
            q.RuleFor(x => x.Label).NotEmpty().When(x => x.Type == "CUSTOM");
        });
    }
}
