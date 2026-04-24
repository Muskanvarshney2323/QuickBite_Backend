using Microsoft.EntityFrameworkCore;

// Alias so "DeliveryAgentEntity" always means the class,
// not the "QuickBite.DeliveryAgent" namespace.
using DeliveryAgentEntity = QuickBite.DeliveryAgent.Domain.Entities.DeliveryAgent;

namespace QuickBite.DeliveryAgent.Infrastructure.Data;

/// <summary>
/// EF Core database context for the Delivery Agent-Service.
/// Has one table: DeliveryAgents.
/// </summary>
public class DeliveryAgentDbContext : DbContext
{
      public DeliveryAgentDbContext(DbContextOptions<DeliveryAgentDbContext> options) : base(options)
      {
      }

      // The DeliveryAgents table.
      public DbSet<DeliveryAgentEntity> DeliveryAgents => Set<DeliveryAgentEntity>();

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DeliveryAgentEntity>(entity =>
            {
                  entity.ToTable("DeliveryAgents");

                  // Primary key.
                  entity.HasKey(x => x.Id);

                  // Required fields with max length limits.
                  entity.Property(x => x.UserId).IsRequired();

                  entity.Property(x => x.FullName)
                    .HasMaxLength(200)
                    .IsRequired();

                  entity.Property(x => x.Phone)
                    .HasMaxLength(20)
                    .IsRequired();

                  // Store the enum as an integer in the database.
                  entity.Property(x => x.VehicleType)
                    .HasConversion<int>()
                    .IsRequired();

                  entity.Property(x => x.VehicleNumber)
                    .HasMaxLength(30)
                    .IsRequired();

                  entity.Property(x => x.CurrentLatitude).IsRequired();
                  entity.Property(x => x.CurrentLongitude).IsRequired();

                  entity.Property(x => x.IsAvailable).IsRequired();
                  entity.Property(x => x.IsVerified).IsRequired();

                  entity.Property(x => x.AvgRating).IsRequired();
                  entity.Property(x => x.TotalDeliveries).IsRequired();
            });
      }
}