
using Domain.Users;

namespace Application.Abstractions.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task SaveChangesAsync();
    Task<User?> GetByFacebookIdAsync(string id);
}
