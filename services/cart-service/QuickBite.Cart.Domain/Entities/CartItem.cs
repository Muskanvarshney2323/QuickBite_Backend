using QuickBite.Cart.Domain.Common;

namespace QuickBite.Cart.Domain.Entities;

/// <summary>
/// Represents a single item inside the cart.
/// </summary>
public class CartItem : BaseEntity
{
    public Guid CartId { get; set; }

    public Guid MenuItemId { get; set; }

    public string MenuItemName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    /// <summary>
    /// Navigation property for related cart.
    /// </summary>
    public Cart? Cart { get; set; }
}