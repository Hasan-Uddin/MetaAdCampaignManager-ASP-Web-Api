using System.Text.Json;
using Domain.Leads;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs.Meta;

internal sealed class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasMaxLength(50);
        builder.Property(l => l.FormId).HasMaxLength(50);
        builder.Property(l => l.AdId).HasMaxLength(50);
        builder.Property(l => l.CampaignId).HasMaxLength(50);
        builder.Property(l => l.AdSetId).HasMaxLength(50);
        builder.Property(l => l.FieldData)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null)!);
    }
}
