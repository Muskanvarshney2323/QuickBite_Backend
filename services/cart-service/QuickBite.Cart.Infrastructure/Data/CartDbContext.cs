using Microsoft.EntityFrameworkCore;
using QuickBite.Cart.Domain.Entities;

namespace QuickBite.Cart.Infrastructure.Data;

/// <summary>
/// Database context for Cart Service.
/// </summary>
public class CartDbContext : DbContext
{
    public CartDbContext(DbContextOptions<CartDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Cart> Carts => Set<Domain.Entities.Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    /// <summary>
    /// Configures entity relationships and column rules.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.Cart>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.UserId)
                  .IsRequired();

            entity.Property(x => x.CreatedAt)
                  .IsRequired();

            entity.Property(x => x.UpdatedAt)
                  .IsRequired();

            entity.HasMany(x => x.CartItems)
                  .WithOne(x => x.Cart!)
                  .HasForeignKey(x => x.CartId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.MenuItemId)
                  .IsRequired();

            entity.Property(x => x.MenuItemName)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(x => x.UnitPrice)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.Property(x => x.Quantity)
                  .IsRequired();
        });
    }
}