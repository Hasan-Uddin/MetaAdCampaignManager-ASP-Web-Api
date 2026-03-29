using Domain.WhatsApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs.WhatsApp;

internal sealed class WhatsAppSettingConfiguration : IEntityTypeConfiguration<WhatsAppSetting>
{
    public void Configure(EntityTypeBuilder<WhatsAppSetting> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.AccessToken).HasMaxLength(500);
        builder.Property(s => s.BusinessAccountId).HasMaxLength(50);
        builder.Property(s => s.PhoneNumberId).HasMaxLength(50);
        builder.Property(s => s.PhoneNumber).HasMaxLength(20);
        builder.Property(s => s.WebhookVerifyToken).HasMaxLength(200);
    }
}
