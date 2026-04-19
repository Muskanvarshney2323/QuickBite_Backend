using Microsoft.EntityFrameworkCore;
using QuickBite.Auth.Application.Interfaces;
using QuickBite.Auth.Domain.Entities;
using QuickBite.Auth.Infrastructure.Context;

namespace QuickBite.Auth.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;

        public UserRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}