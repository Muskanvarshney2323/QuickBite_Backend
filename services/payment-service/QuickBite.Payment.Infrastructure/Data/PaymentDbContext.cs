using Microsoft.EntityFrameworkCore;
using QuickBite.Payment.Domain.Entities;

namespace QuickBite.Payment.Infrastructure.Data;

/// <summary>
/// EF Core database context for the Payment / Wallet service.
/// </summary>
public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Payment> Payments => Set<Domain.Entities.Payment>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletStatement> WalletStatements => Set<WalletStatement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.Payment>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.OrderId).IsRequired();
            entity.Property(x => x.CustomerId).IsRequired();

            entity.Property(x => x.Amount).HasColumnType("numeric(18,2)").IsRequired();

            entity.Property(x => x.Status).HasConversion<int>().IsRequired();
            entity.Property(x => x.Mode).HasConversion<int>().IsRequired();

            entity.Property(x => x.TransactionId).HasMaxLength(100);
            entity.Property(x => x.Currency).HasMaxLength(8).IsRequired();

            // One payment per order.
            entity.HasIndex(x => x.OrderId).IsUnique();
            entity.HasIndex(x => x.CustomerId);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.TransactionId);
            entity.HasIndex(x => x.PaidAt);
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.CustomerId).IsRequired();
            entity.Property(x => x.Balance).HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.UpdatedAt).IsRequired();

            // One wallet per customer.
            entity.HasIndex(x => x.CustomerId).IsUnique();

            entity.HasMany(x => x.WalletStatements)
                  .WithOne(x => x.Wallet!)
                  .HasForeignKey(x => x.WalletId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WalletStatement>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Type).HasConversion<int>().IsRequired();
            entity.Property(x => x.Amount).HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(x => x.BalanceAfter).HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(x => x.Reference).HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(300);
            entity.Property(x => x.CreatedAt).IsRequired();

            entity.HasIndex(x => new { x.WalletId, x.CreatedAt });
        });
    }
}
