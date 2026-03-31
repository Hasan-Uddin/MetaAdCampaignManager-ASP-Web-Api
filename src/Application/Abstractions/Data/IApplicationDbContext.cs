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
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Ad> Ads { get; }
    DbSet<AdSet> AdSets { get; }
    DbSet<Campaign> Campaigns { get; }
    DbSet<Lead> Leads { get; }
    DbSet<Form> Forms { get; }
    DbSet<MetaSetting> MetaSettings { get; }
    DbSet<FormQuestion> FormQuestions { get; }
    DbSet<FormTemplate> FormTemplates { get; }
    DbSet<WhatsAppSetting> WhatsAppSettings { get; }
    DbSet<Conversation> Conversations { get; }
    DbSet<Message> Messages { get; }
    DbSet<WhatsAppCallConfig> WhatsAppCallConfigs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
