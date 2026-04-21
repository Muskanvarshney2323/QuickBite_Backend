namespace QuickBite.Cart.Domain.Common;

/// <summary>
/// Base entity class used to keep common properties shared by multiple entities.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}