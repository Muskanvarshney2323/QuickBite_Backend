using QuickBite.Auth.Domain.Entities;

namespace QuickBite.Auth.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid userId);
        Task<bool> ExistsByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task SaveChangesAsync();
    }
}
