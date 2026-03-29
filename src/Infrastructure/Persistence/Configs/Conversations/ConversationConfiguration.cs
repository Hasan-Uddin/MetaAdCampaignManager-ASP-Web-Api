using Domain.Conversations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs.Conversations;

internal sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.CustomerPhone).HasMaxLength(20);
        builder.Property(c => c.CustomerName).HasMaxLength(200);
        builder.Property(c => c.LastMessage).HasMaxLength(1000);
        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId);
    }
}
