using FluentValidation;

namespace Application.Features.WhatsApp.CallConfig.Update;

internal sealed class UpdateCallConfigCommandValidator : AbstractValidator<UpdateCallConfigCommand>
{
    private static readonly string[] ValidModes = ["Always", "BusinessHours", "Disabled"];
    private static readonly string[] ValidDays = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

    public UpdateCallConfigCommandValidator()
    {
        RuleFor(x => x.CallHoursMode).Must(m => ValidModes.Contains(m))
            .WithMessage("CallHoursMode must be Always, BusinessHours, or Disabled.");

        When(x => x.CallHoursMode == "BusinessHours", () =>
        {
            RuleFor(x => x.BusinessHours).NotEmpty()
                .WithMessage("BusinessHours required when mode is BusinessHours.");

            RuleForEach(x => x.BusinessHours).ChildRules(h =>
            {
                h.RuleFor(x => x.Day).Must(d => ValidDays.Contains(d));
                h.RuleFor(x => x.OpenTime).NotEmpty();
                h.RuleFor(x => x.CloseTime).NotEmpty();
            });
        });
    }
}
