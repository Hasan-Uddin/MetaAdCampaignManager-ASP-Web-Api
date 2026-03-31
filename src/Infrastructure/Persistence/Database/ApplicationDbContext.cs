using Application.Abstractions.Data;
using Domain.Ads;
using Domain.AdSets;
using Domain.Campaigns;
using Domain.Conversations;
using Domain.FormQuestions;
using Domain.Forms;
using Domain.FormTemplates;
using Domain.Leads;
using Domain.Messages;
using Domain.MetaSettings;
using Domain.Users;
using Domain.WhatsApp;
using Domain.WhatsAppCall;
using Infrastructure.Persistence.DomainEvents;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Persistence.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Ad> Ads { get; set; }
    public DbSet<AdSet> AdSets { get; set; }
    public DbSet<Lead> Leads { get; set; }
    public DbSet<Form> Forms { get; set; }
    public DbSet<MetaSetting> MetaSettings { get; set; }
    public DbSet<FormQuestion> FormQuestions { get; set; }
    public DbSet<FormTemplate> FormTemplates { get; set; }
    public DbSet<WhatsAppSetting> WhatsAppSettings { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<WhatsAppCallConfig> WhatsAppCallConfigs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Default);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // When should you publish domain events?
        //
        // 1. BEFORE calling SaveChangesAsync
        //     - domain events are part of the same transaction
        //     - immediate consistency
        // 2. AFTER calling SaveChangesAsync
        //     - domain events are a separate transaction
        //     - eventual consistency
        //     - handlers can fail

        int result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();

        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                List<IDomainEvent> domainEvents = entity.DomainEvents;

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

        await domainEventsDispatcher.DispatchAsync(domainEvents);
    }
}
