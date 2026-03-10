using Domain.Ads;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs.Meta;

internal sealed class AdConfiguration : IEntityTypeConfiguration<Ad>
{
    public void Configure(EntityTypeBuilder<Ad> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasMaxLength(50);
        builder.Property(a => a.Name).HasMaxLength(200);
        builder.Property(a => a.Status).HasMaxLength(50);
        builder.Property(a => a.AdSetId).HasMaxLength(50);
        builder.Property(a => a.CampaignId).HasMaxLength(50);
    }
}
