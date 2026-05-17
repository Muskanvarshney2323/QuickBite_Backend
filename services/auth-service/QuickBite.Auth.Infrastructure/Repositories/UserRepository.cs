// Entity Framework Core for database queries
using Microsoft.EntityFrameworkCore;

// Imports IUserRepository interface
using QuickBite.Auth.Application.Interfaces;

// Imports User entity
using QuickBite.Auth.Domain.Entities;

// Imports AuthDbContext for database operations
using QuickBite.Auth.Infrastructure.Context;

// Namespace for repository classes
namespace QuickBite.Auth.Infrastructure.Repositories
{
    // UserRepository: Implements data access logic for User entity
    // Handles all database operations for User (CRUD operations)
    public class UserRepository : IUserRepository
    {
        // Database context for querying Users table
        private readonly AuthDbContext _context;

        // ========================= CONSTRUCTOR =========================

        // Constructor with Dependency Injection
        // Receives AuthDbContext from ASP.NET DI container
        public UserRepository(AuthDbContext context)
        {
            // Store context reference for database operations
            _context = context;
        }

        // ========================= GET USER BY EMAIL METHOD =========================

        // Method: GetByEmailAsync - Finds user by email address
        // Parameter: email - Email to search for
        // Returns: User object if found, null if not found
        public async Task<User?> GetByEmailAsync(string email)
        {
            // Query Users table asynchronously
            return await _context.Users
                // Compare emails case-insensitively (convert both to lowercase)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        // ========================= GET USER BY ID METHOD =========================

        // Method: GetByIdAsync - Finds user by unique ID (GUID)
        // Parameter: userId - User's unique identifier
        // Returns: User object if found, null if not found
        public async Task<User?> GetByIdAsync(Guid userId)
        {
            // Query Users table asynchronously
            return await _context.Users
                // Find user where UserId matches provided ID
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        // ========================= CHECK IF EMAIL EXISTS METHOD =========================

        // Method: ExistsByEmailAsync - Checks if email already exists
        // Parameter: email - Email to check
        // Returns: True if user exists, False if not
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            // Query Users table asynchronously
            return await _context.Users
                // Check if any user has this email (case-insensitive)
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        // ========================= ADD USER METHOD =========================

        // Method: AddAsync - Adds new user to database (in memory)
        // Parameter: user - User object to add
        // Note: Changes not saved until SaveChangesAsync is called
        public async Task AddAsync(User user)
        {
            // Add user to Users DbSet (in memory, not yet committed to database)
            await _context.Users.AddAsync(user);
        }

        // ========================= UPDATE USER METHOD =========================

        // Method: UpdateAsync - Updates existing user in database (in memory)
        // Parameter: user - User object with updated values
        // Note: Changes not saved until SaveChangesAsync is called
        public Task UpdateAsync(User user)
        {
            // Mark user as modified in DbSet (in memory, not yet committed to database)
            _context.Users.Update(user);

            // Return completed task (no actual database commit)
            return Task.CompletedTask;
        }

        // ========================= SAVE CHANGES METHOD =========================

        // Method: SaveChangesAsync - Commits all changes to database
        // Saves all pending Add, Update, Delete operations
        public async Task SaveChangesAsync()
        {
            // Execute all pending database changes (INSERT, UPDATE, DELETE statements)
            await _context.SaveChangesAsync();
        }
    }
}
