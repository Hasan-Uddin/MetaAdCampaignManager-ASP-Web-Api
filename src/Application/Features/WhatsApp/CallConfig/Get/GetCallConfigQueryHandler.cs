using System.Globalization;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.WhatsApp;
using Domain.WhatsApp;
using Domain.WhatsAppCall;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.WhatsApp.CallConfig.Get;

internal sealed class GetCallConfigQueryHandler(
    IApplicationDbContext context,
    IWhatsAppCallingService callingService) : IQueryHandler<GetCallConfigQuery, CallConfigResponse>
{
    public async Task<Result<CallConfigResponse>> Handle(GetCallConfigQuery query, CancellationToken cancellationToken)
    {
        WhatsAppSetting? settings = await context.WhatsAppSettings
            .FirstOrDefaultAsync(s => s.UserId == query.UserId, cancellationToken);

        if (settings is null)
        {
            return Result.Failure<CallConfigResponse>(
                Error.Failure("WhatsApp.NotConfigured", "WhatsApp not configured."));
        }

        // Try to get live from Meta API
        Result<WhatsAppCallConfigSnapshot> metaResult = await callingService.GetCallSettingsAsync(
            settings.PhoneNumberId, settings.AccessToken, cancellationToken);

        if (metaResult.IsSuccess)
        {
            // Upsert to DB
            WhatsAppCallConfig? config = await context.WhatsAppCallConfigs
                .FirstOrDefaultAsync(c => c.UserId == query.UserId, cancellationToken);

            if (config is null)
            {
                config = new WhatsAppCallConfig
                {
                    Id = Guid.NewGuid(),
                    UserId = query.UserId,
                    PhoneNumberId = settings.PhoneNumberId
                };
                context.WhatsAppCallConfigs.Add(config);
            }

            config.CallingEnabled = metaResult.Value.CallingEnabled;
            config.InboundCallsEnabled = metaResult.Value.InboundCallsEnabled;
            config.CallbackRequestsEnabled = metaResult.Value.CallbackRequestsEnabled;
            config.CallHoursMode = Enum.Parse<CallHoursMode>(
                metaResult.Value.CallHoursMode, ignoreCase: true);
            config.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);

            return new CallConfigResponse
            {
                CallingEnabled = metaResult.Value.CallingEnabled,
                InboundCallsEnabled = metaResult.Value.InboundCallsEnabled,
                CallbackRequestsEnabled = metaResult.Value.CallbackRequestsEnabled,
                CallHoursMode = metaResult.Value.CallHoursMode
            };
        }

        // Fallback to DB
        WhatsAppCallConfig? dbConfig = await context.WhatsAppCallConfigs
            .FirstOrDefaultAsync(c => c.UserId == query.UserId, cancellationToken);

        if (dbConfig is null)
        {
            return Result.Failure<CallConfigResponse>(
                Error.Failure("WhatsApp.CallConfig.NotFound", "Call config not found."));
        }

        return new CallConfigResponse
        {
            CallingEnabled = dbConfig.CallingEnabled,
            InboundCallsEnabled = dbConfig.InboundCallsEnabled,
            CallbackRequestsEnabled = dbConfig.CallbackRequestsEnabled,
            CallHoursMode = dbConfig.CallHoursMode.ToString(),
            BusinessHours = dbConfig.BusinessHours.Select(h => new CallBusinessHourResponse
            {
                Day = h.Day.ToString(),
                OpenTime = h.OpenTime.ToString(CultureInfo.InvariantCulture),
                CloseTime = h.CloseTime.ToString(CultureInfo.InvariantCulture)
            }).ToList()
        };
    }
}
