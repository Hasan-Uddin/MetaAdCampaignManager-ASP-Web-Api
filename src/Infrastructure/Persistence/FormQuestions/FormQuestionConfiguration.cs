using Domain.FormQuestions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.FormQuestions;

internal sealed class FormQuestionConfiguration : IEntityTypeConfiguration<FormQuestion>
{
    public void Configure(EntityTypeBuilder<FormQuestion> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasMaxLength(70);
        
        builder.Property(f => f.Type)
            .HasMaxLength(50);

        builder.Property(f => f.Label)
            .HasMaxLength(50);
    }
}
