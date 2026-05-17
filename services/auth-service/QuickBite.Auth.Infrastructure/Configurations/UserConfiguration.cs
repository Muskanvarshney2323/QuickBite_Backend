// Entity Framework Core for database configuration
using Microsoft.EntityFrameworkCore;

// For configuring entity properties and constraints
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// Imports User entity
using QuickBite.Auth.Domain.Entities;

// Namespace for entity configurations
namespace QuickBite.Auth.Infrastructure.Configurations
{
       // UserConfiguration: Defines how User entity maps to database table
       // Implements IEntityTypeConfiguration for User
       public class UserConfiguration : IEntityTypeConfiguration<User>
       {
              // ========================= CONFIGURE METHOD =========================

              // Method: Configure - Sets up database table structure and constraints
              // Parameter: builder - Used to configure User entity properties
              public void Configure(EntityTypeBuilder<User> builder)
              {
                     // Map User class to "Users" table in PostgreSQL database
                     builder.ToTable("Users");

                     // ========================= PRIMARY KEY =========================

                     // Set UserId as primary key (unique identifier for each row)
                     builder.HasKey(user => user.UserId);

                     // ========================= COLUMN CONFIGURATIONS =========================

                     // Configure FullName column
                     builder.Property(user => user.FullName)
                            // Column must have a value (NOT NULL in database)
                            .IsRequired()
                            // Maximum 100 characters (VARCHAR(100))
                            .HasMaxLength(100);

                     // Configure Email column
                     builder.Property(user => user.Email)
                            // Column must have a value (NOT NULL)
                            .IsRequired()
                            // Maximum 100 characters (VARCHAR(100))
                            .HasMaxLength(100);

                     // Configure PasswordHash column
                     builder.Property(user => user.PasswordHash)
                            // Column must have a value (NOT NULL)
                            .IsRequired();
                     // Note: No length limit as bcrypt hashes are long

                     // Configure Phone column
                     builder.Property(user => user.Phone)
                            // Column must have a value (NOT NULL)
                            .IsRequired()
                            // Maximum 20 characters (VARCHAR(20))
                            .HasMaxLength(20);

                     // Configure Role column
                     builder.Property(user => user.Role)
                            // Column must have a value (NOT NULL)
                            .IsRequired()
                            // Maximum 30 characters (VARCHAR(30))
                            .HasMaxLength(30);

                     // Configure IsActive column
                     builder.Property(user => user.IsActive)
                            // Column must have a value (NOT NULL)
                            .IsRequired();
                     // Type: boolean in database

                     // Configure CreatedAt column
                     builder.Property(user => user.CreatedAt)
                            // Column must have a value (NOT NULL)
                            .IsRequired();
                     // Type: timestamp in database

                     // ========================= INDEXES =========================

                     // Create unique index on Email column
                     builder.HasIndex(user => user.Email)
                            // Email must be unique across all users
                            // Prevents duplicate email registrations
                            .IsUnique();
              }
       }
}