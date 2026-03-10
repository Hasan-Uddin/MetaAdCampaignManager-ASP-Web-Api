using FluentValidation;

namespace Application.Features.Meta.MetaSettings.Create;

internal sealed class CreateMetaSettingsCommandValidator : AbstractValidator<CreateeMetaSettingsCommand>
{
    public CreateMetaSettingsCommandValidator()
    {
        RuleFor(x => x.AppId).NotEmpty();
        RuleFor(x => x.AppSecret).NotEmpty();
        RuleFor(x => x.UserToken).NotEmpty();
        RuleFor(x => x.WebhookVerifyToken).NotEmpty();
    }
}
