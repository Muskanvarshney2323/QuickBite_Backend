using Microsoft.EntityFrameworkCore;
using QuickBite.Order.Domain.Entities;

namespace QuickBite.Order.Infrastructure.Data;

/// <summary>
/// EF Core database context for the Order-Service.
/// </summary>
public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Order> Orders => Set<Domain.Entities.Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.Order>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.CustomerId).IsRequired();
            entity.Property(x => x.RestaurantId).IsRequired();

            entity.Property(x => x.TotalAmount).HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(x => x.Discount).HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(x => x.FinalAmount).HasColumnType("numeric(18,2)").IsRequired();

            // Persist enums as ints so the database is readable across providers.
            entity.Property(x => x.ModeOfPayment).HasConversion<int>().IsRequired();
            entity.Property(x => x.OrderStatus).HasConversion<int>().IsRequired();

            entity.Property(x => x.OrderDate).IsRequired();

            entity.Property(x => x.DeliveryAddress)
                  .HasMaxLength(500)
                  .IsRequired();

            entity.Property(x => x.SpecialInstructions).HasMaxLength(1000);

            // Common lookup paths.
            entity.HasIndex(x => x.CustomerId);
            entity.HasIndex(x => x.RestaurantId);
            entity.HasIndex(x => x.OrderStatus);
            entity.HasIndex(x => x.DeliveryAgentId);
            entity.HasIndex(x => x.OrderDate);

            entity.HasMany(x => x.OrderItems)
                  .WithOne(x => x.Order!)
                  .HasForeignKey(x => x.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.MenuItemId).IsRequired();

            entity.Property(x => x.Name)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(x => x.Price).HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(x => x.Quantity).IsRequired();
            entity.Property(x => x.Customization).HasMaxLength(500);
        });
    }
}
