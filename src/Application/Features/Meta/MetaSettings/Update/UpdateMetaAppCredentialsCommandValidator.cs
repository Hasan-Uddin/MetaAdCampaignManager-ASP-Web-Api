using FluentValidation;

namespace Application.Features.Meta.MetaSettings.Update;

internal sealed class UpdateMetaAppCredentialsCommandValidator : AbstractValidator<UpdateMetaAppCredentialsCommand>
{
    public UpdateMetaAppCredentialsCommandValidator()
    {
        RuleFor(x => x.AppId).NotEmpty();
        RuleFor(x => x.AppSecret).NotEmpty();
    }
}
