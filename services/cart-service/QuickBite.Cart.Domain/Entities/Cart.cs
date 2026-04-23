using QuickBite.Cart.Domain.Common;

namespace QuickBite.Cart.Domain.Entities;

/// <summary>
/// Represents a customer's single active shopping cart.
/// Each cart is bound to exactly one restaurant to enforce
/// single-restaurant ordering (a customer cannot mix items
/// from multiple restaurants in the same cart).
/// </summary>
public class Cart : BaseEntity
{
    /// <summary>
    /// Identifier of the customer who owns this cart.
    /// Each customer has at most one active cart at a time.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// The restaurant this cart is currently bound to.
    /// All CartItems must belong to this restaurant.
    /// </summary>
    public Guid RestaurantId { get; set; }

    /// <summary>
    /// Running total of the cart (sum of Price * Quantity for each item).
    /// Recomputed by the service whenever cart items change.
    /// </summary>
    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Items currently in the cart (0..* CartItems).
    /// </summary>
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
