using Microsoft.EntityFrameworkCore;

// Alias so "NotificationEntity" always means the class,
// not the "QuickBite.Notification" namespace.
using NotificationEntity = QuickBite.Notification.Domain.Entities.Notification;

namespace QuickBite.Notification.Infrastructure.Data;

/// <summary>
/// EF Core database context for the Notification-Service.
/// Has one table: Notifications.
/// </summary>
public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    // The Notifications table.
    public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NotificationEntity>(entity =>
        {
            entity.ToTable("Notifications");

            // Primary key.
            entity.HasKey(x => x.Id);

            // Required fields with max length limits.
            entity.Property(x => x.RecipientId).IsRequired();

            // Store enums as integers.
            entity.Property(x => x.Type)
                  .HasConversion<int>()
                  .IsRequired();

            entity.Property(x => x.Channel)
                  .HasConversion<int>()
                  .IsRequired();

            entity.Property(x => x.Title)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(x => x.Message)
                  .HasMaxLength(1000)
                  .IsRequired();

            entity.Property(x => x.RelatedType)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(x => x.IsRead).IsRequired();
            entity.Property(x => x.SentAt).IsRequired();
        });
    }
}
