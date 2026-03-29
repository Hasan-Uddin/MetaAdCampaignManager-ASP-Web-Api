using FluentValidation;

namespace Application.Features.WhatsApp.Conversations.Create;

internal sealed class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationCommandValidator()
    {
        RuleFor(x => x.CustomerPhone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(200);
    }
}
