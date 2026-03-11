using Domain.MetaSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs.MetaSettings;

internal sealed class MetaSettingsConfiguration : IEntityTypeConfiguration<MetaSetting>
{
    public void Configure(EntityTypeBuilder<MetaSetting> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.AccessToken).HasMaxLength(500);
        builder.Property(s => s.PageId).HasMaxLength(50);
        builder.Property(s => s.AdAccountId).HasMaxLength(50);
        builder.Property(s => s.PageAccessToken).HasMaxLength(500);
        builder.Property(s => s.AccessTokenExpiresAt).IsRequired();
        builder.Property(s => s.UserId).IsRequired();
        builder.HasOne(s => s.User)
            .WithOne(u => u.MetaSetting)
            .HasForeignKey<MetaSetting>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
