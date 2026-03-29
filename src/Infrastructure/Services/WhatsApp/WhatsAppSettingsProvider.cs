using Application.Abstractions.Data;
using Application.Abstractions.WhatsApp;
using Domain.WhatsApp;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Services.WhatsApp;

internal sealed class WhatsAppSettingsProvider(IApplicationDbContext context) : IWhatsAppSettingsProvider
{
    public async Task<Result<WhatsAppSettingsSnapshot>> GetAsync(Guid userId, CancellationToken ct = default)
    {
        WhatsAppSetting? s = await context.WhatsAppSettings
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

        if (s is null || string.IsNullOrWhiteSpace(s.AccessToken))
        {
            return Result.Failure<WhatsAppSettingsSnapshot>(
                Error.Failure("WhatsApp.NotConfigured", "WhatsApp is not configured. Complete Meta login first."));
        }

        return new WhatsAppSettingsSnapshot(
            s.AccessToken,
            s.BusinessAccountId,
            s.PhoneNumberId,
            s.PhoneNumber);
    }
}
