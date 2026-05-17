// Entity Framework Core ORM for database operations
using Microsoft.EntityFrameworkCore;

// Imports User entity from Domain layer
using QuickBite.Auth.Domain.Entities;

// Namespace for database context
namespace QuickBite.Auth.Infrastructure.Context
{
    // AuthDbContext: Database connection bridge using Entity Framework Core
    // Inherits from DbContext which manages database operations
    public class AuthDbContext : DbContext
    {
        // Constructor with Dependency Injection
        // Receives database configuration options from ASP.NET DI container
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
            // Pass configuration options to parent DbContext
        }

        // ========================= DATABASE SETS =========================

        // DbSet<User>: Maps User C# class to Users table in PostgreSQL database
        // Used to query, add, update, delete users from database
        public DbSet<User> Users { get; set; }

        // ========================= MODEL CONFIGURATION =========================

        // OnModelCreating: Called when Entity Framework builds the database model
        // Allows configuration of entity properties and relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call parent implementation to apply default configurations
            base.OnModelCreating(modelBuilder);

            // Apply all configuration classes from this assembly
            // Looks for IEntityTypeConfiguration implementations (like UserConfiguration)
            // This automatically applies column mappings, constraints, indexes defined in configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
        }
    }
}