using Domain.MetaSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs.MetaSettings;

internal sealed class MetaSettingsConfiguration : IEntityTypeConfiguration<MetaSetting>
{
    public void Configure(EntityTypeBuilder<MetaSetting> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.AppId).HasMaxLength(100);
        builder.Property(s => s.AppSecret).HasMaxLength(200);
        builder.Property(s => s.AccessToken).HasMaxLength(500);
        builder.Property(s => s.PageId).HasMaxLength(50);
        builder.Property(s => s.AdAccountId).HasMaxLength(50);
        builder.Property(s => s.WebhookVerifyToken).HasMaxLength(200);
        builder.Property(s => s.PageAccessToken).HasMaxLength(500);
    }
}
