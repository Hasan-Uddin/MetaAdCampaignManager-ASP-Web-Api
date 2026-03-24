using Domain.Leads;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs.Leads;

internal sealed class StructuredLeadConfiguration : IEntityTypeConfiguration<StructuredLead>
{
    public void Configure(EntityTypeBuilder<StructuredLead> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasMaxLength(50);
        builder.Property(l => l.FormId).HasMaxLength(50);
        builder.Property(l => l.AdId).HasMaxLength(50);
        builder.Property(l => l.CampaignId).HasMaxLength(50);
        builder.Property(l => l.AdSetId).HasMaxLength(50);

        builder.OwnsOne(l => l.FieldData, fd =>
        {
            fd.Property(f => f.FirstName).HasMaxLength(100);
            fd.Property(f => f.LastName).HasMaxLength(100);
            fd.Property(f => f.Country).HasMaxLength(100);
            fd.Property(f => f.Phone).HasMaxLength(50);
            fd.Property(f => f.Extra)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null)!);
        });
    }
}
