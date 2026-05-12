using Microsoft.EntityFrameworkCore;

// Alias so "ReviewEntity" always means the class,
// not the "QuickBite.Review" namespace.
using ReviewEntity = QuickBite.Review.Domain.Entities.Review;

namespace QuickBite.Review.Infrastructure.Data;

/// <summary>
/// EF Core database context for the Review-Service.
/// Has one table: Reviews.
/// </summary>
public class ReviewDbContext : DbContext
{
    public ReviewDbContext(DbContextOptions<ReviewDbContext> options) : base(options)
    {
    }

    // The Reviews table.
    public DbSet<ReviewEntity> Reviews => Set<ReviewEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ReviewEntity>(entity =>
        {
            entity.ToTable("Reviews");

            // Primary key.
            entity.HasKey(x => x.Id);

            // Required fields.
            entity.Property(x => x.OrderId).IsRequired();
            entity.Property(x => x.CustomerId).IsRequired();
            entity.Property(x => x.RestaurantId).IsRequired();
            entity.Property(x => x.AgentId).IsRequired();

            entity.Property(x => x.FoodRating).IsRequired();
            entity.Property(x => x.DeliveryRating).IsRequired();

            entity.Property(x => x.Comment)
                  .HasMaxLength(1000);

            entity.Property(x => x.ReviewDate).IsRequired();
            entity.Property(x => x.IsVerified).IsRequired();

            // One review per order: unique index on OrderId.
            entity.HasIndex(x => x.OrderId).IsUnique();
        });
    }
}
