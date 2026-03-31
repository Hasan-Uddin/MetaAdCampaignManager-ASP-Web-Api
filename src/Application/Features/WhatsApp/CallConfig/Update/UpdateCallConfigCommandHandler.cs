using System.Globalization;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.WhatsApp;
using Domain.WhatsApp;
using Domain.WhatsAppCall;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.WhatsApp.CallConfig.Update;

internal sealed class UpdateCallConfigCommandHandler(
    IApplicationDbContext context,
    IWhatsAppCallingService callingService) : ICommandHandler<UpdateCallConfigCommand>
{
    public async Task<Result> Handle(UpdateCallConfigCommand command, CancellationToken cancellationToken)
    {
        WhatsAppSetting? settings = await context.WhatsAppSettings
            .FirstOrDefaultAsync(s => s.UserId == command.UserId, cancellationToken);

        if (settings is null)
        {
            return Result.Failure(Error.Failure("WhatsApp.NotConfigured", "WhatsApp not configured."));
        }

        // Push to Meta first
        var updateRequest = new UpdateCallSettingsRequest(
            command.CallingEnabled,
            command.InboundCallsEnabled,
            command.CallbackRequestsEnabled,
            command.CallHoursMode,
            command.BusinessHours.Select(h => new CallHourEntry(h.Day, h.OpenTime, h.CloseTime)).ToList());

        Result metaResult = await callingService.UpdateCallSettingsAsync(
            settings.PhoneNumberId, settings.AccessToken, updateRequest, cancellationToken);

        if (metaResult.IsFailure)
        {
            return Result.Failure(metaResult.Error);
        }

        // Save to DB
        WhatsAppCallConfig? config = await context.WhatsAppCallConfigs
            .FirstOrDefaultAsync(c => c.UserId == command.UserId, cancellationToken);

        if (config is null)
        {
            config = new WhatsAppCallConfig
            {
                Id = Guid.NewGuid(),
                UserId = command.UserId,
                PhoneNumberId = settings.PhoneNumberId
            };
            context.WhatsAppCallConfigs.Add(config);
        }

        config.CallingEnabled = command.CallingEnabled;
        config.InboundCallsEnabled = command.InboundCallsEnabled;
        config.CallbackRequestsEnabled = command.CallbackRequestsEnabled;
        config.CallHoursMode = Enum.Parse<CallHoursMode>(command.CallHoursMode, ignoreCase: true);
        config.BusinessHours = command.BusinessHours.Select(h => new CallBusinessHour
        {
            Day = Enum.Parse<DayOfWeek>(h.Day, ignoreCase: true),
            OpenTime = TimeOnly.Parse(h.OpenTime, CultureInfo.InvariantCulture),
            CloseTime = TimeOnly.Parse(h.CloseTime, CultureInfo.InvariantCulture)
        }).ToList();
        config.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
