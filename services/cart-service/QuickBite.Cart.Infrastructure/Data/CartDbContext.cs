using Microsoft.EntityFrameworkCore;
using QuickBite.Cart.Domain.Entities;

namespace QuickBite.Cart.Infrastructure.Data;

/// <summary>
/// EF Core database context for the Cart-Service.
/// </summary>
public class CartDbContext : DbContext
{
    public CartDbContext(DbContextOptions<CartDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Cart> Carts => Set<Domain.Entities.Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.Cart>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.CustomerId).IsRequired();
            entity.Property(x => x.RestaurantId).IsRequired();

            // numeric(18,2) is the PostgreSQL equivalent of decimal(18,2).
            entity.Property(x => x.TotalPrice)
                  .HasColumnType("numeric(18,2)")
                  .IsRequired();

            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt).IsRequired();

            // One active cart per customer.
            entity.HasIndex(x => x.CustomerId).IsUnique();

            entity.HasMany(x => x.CartItems)
                  .WithOne(x => x.Cart!)
                  .HasForeignKey(x => x.CartId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.MenuItemId).IsRequired();

            entity.Property(x => x.Name)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(x => x.Price)
                  .HasColumnType("numeric(18,2)")
                  .IsRequired();

            entity.Property(x => x.Quantity).IsRequired();

            entity.Property(x => x.Customization)
                  .HasMaxLength(500);
        });
    }
}
