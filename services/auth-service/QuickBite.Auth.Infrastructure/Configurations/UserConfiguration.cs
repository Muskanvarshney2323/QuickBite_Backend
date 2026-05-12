using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickBite.Auth.Domain.Entities;

namespace QuickBite.Auth.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(user => user.UserId);

            builder.Property(user => user.FullName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(user => user.Email)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(user => user.PasswordHash)
                   .IsRequired();

            builder.Property(user => user.Phone)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(user => user.Role)
                   .IsRequired()
                   .HasMaxLength(30);

            builder.Property(user => user.IsActive)
                   .IsRequired();

            builder.Property(user => user.CreatedAt)
                   .IsRequired();

            builder.HasIndex(user => user.Email)
                   .IsUnique();
        }
    }
}