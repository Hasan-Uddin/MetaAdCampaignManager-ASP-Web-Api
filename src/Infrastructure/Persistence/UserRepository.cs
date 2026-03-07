using Application.Abstractions.Data;
using Application.Abstractions.Interfaces;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class UserRepository(IApplicationDbContext context) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
        => await context.Users.FirstOrDefaultAsync(x => x.Email == email);

    public async Task<User?> GetByIdAsync(Guid id)
        => await context.Users.FirstOrDefaultAsync(x => x.Id == id);

    public async Task AddAsync(User user)
        => await context.Users.AddAsync(user);

    public async Task SaveChangesAsync()
        => await context.SaveChangesAsync();
}
