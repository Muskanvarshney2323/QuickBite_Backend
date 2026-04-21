using QuickBite.Cart.Domain.Common;

namespace QuickBite.Cart.Domain.Entities;

/// <summary>
/// Represents a customer's shopping cart.
/// One customer will have one active cart.
/// </summary>
public class Cart : BaseEntity
{
    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}