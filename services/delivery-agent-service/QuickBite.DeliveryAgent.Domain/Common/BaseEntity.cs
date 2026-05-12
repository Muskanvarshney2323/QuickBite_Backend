namespace QuickBite.DeliveryAgent.Domain.Common;

/// <summary>
/// Base entity class shared by all entities in the DeliveryAgent domain.
/// Provides a unique Guid identifier out of the box.
/// </summary>
public abstract class BaseEntity
{
    // Every entity gets a new Guid by default so we never insert a row without an id.
    public Guid Id { get; set; } = Guid.NewGuid();
}
