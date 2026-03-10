using Domain.Ads;
using Domain.AdSets;
using Domain.Campaigns;
using Domain.FormQuestions;
using Domain.Forms;
using Domain.Leads;
using Domain.MetaSettings;
using Domain.Users;
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
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
