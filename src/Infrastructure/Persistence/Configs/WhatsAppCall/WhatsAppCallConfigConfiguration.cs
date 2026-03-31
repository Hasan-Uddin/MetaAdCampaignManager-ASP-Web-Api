using System.Text.Json;
using Domain.WhatsAppCall;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs.WhatsAppCall;

internal sealed class WhatsAppCallConfigConfiguration : IEntityTypeConfiguration<WhatsAppCallConfig>
{
    public void Configure(EntityTypeBuilder<WhatsAppCallConfig> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.PhoneNumberId).HasMaxLength(50);
        builder.Property(c => c.CallHoursMode).HasConversion<string>();
        builder.Property(c => c.BusinessHours)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<CallBusinessHour>>(v, (JsonSerializerOptions?)null)!);
    }
}
