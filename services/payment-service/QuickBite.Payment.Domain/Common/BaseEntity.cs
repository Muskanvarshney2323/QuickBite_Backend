namespace QuickBite.Payment.Domain.Common;

/// <summary>
/// Base entity class shared by all entities in the Payment domain.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}
