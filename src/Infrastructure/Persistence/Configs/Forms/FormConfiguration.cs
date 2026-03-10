using Domain.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs.Forms;

internal sealed class FormConfiguration : IEntityTypeConfiguration<Form>
{
    public void Configure(EntityTypeBuilder<Form> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).HasMaxLength(50);
        builder.Property(f => f.PageId).HasMaxLength(50);
        builder.Property(f => f.Name).HasMaxLength(200);
        builder.Property(f => f.Locale).HasMaxLength(20);
        builder.Property(f => f.Status).HasMaxLength(50);
        builder.Property(f => f.PrivacyPolicyUrl).HasMaxLength(500);
        builder.Property(f => f.PrivacyPolicyLinkText).HasMaxLength(200);
        builder.Property(f => f.FollowUpActionUrl).HasMaxLength(500);
        builder.HasMany(f => f.Questions)
            .WithOne(q => q.Form)
            .HasForeignKey(q => q.FormId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
