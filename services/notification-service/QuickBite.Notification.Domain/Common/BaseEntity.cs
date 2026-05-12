namespace QuickBite.Notification.Domain.Common;

/// <summary>
/// Base entity class shared by all entities in the Notification domain.
/// Provides a unique Guid identifier out of the box.
/// </summary>
public abstract class BaseEntity
{
    // Every entity gets a new Guid by default.
    public Guid Id { get; set; } = Guid.NewGuid();
}
