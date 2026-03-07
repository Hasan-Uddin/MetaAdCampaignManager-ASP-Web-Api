using Domain.Meta;
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
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
