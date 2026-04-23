using QuickBite.Cart.Domain.Common;

namespace QuickBite.Cart.Domain.Entities;

/// <summary>
/// Represents a single item inside a cart.
/// Captures the menu item reference, a price snapshot taken
/// at the time of add, the quantity, and any customisation notes.
/// </summary>
public class CartItem : BaseEntity
{
    /// <summary>
    /// Foreign key to the parent cart.
    /// </summary>
    public Guid CartId { get; set; }

    /// <summary>
    /// Reference to the MenuItem (from Menu-Service) this line represents.
    /// </summary>
    public Guid MenuItemId { get; set; }

    /// <summary>
    /// Name of the menu item at the time it was added.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Price snapshot at the time the item was added to the cart.
    /// Subsequent menu price changes do not affect existing cart rows.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// How many units of this menu item the customer wants.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Free-text customisation notes for this line
    /// (e.g. "no onions", "extra cheese"). Optional.
    /// </summary>
    public string? Customization { get; set; }

    /// <summary>
    /// Navigation property for the parent cart.
    /// </summary>
    public Cart? Cart { get; set; }
}
