using QuickBite.Order.Domain.Common;

namespace QuickBite.Order.Domain.Entities;

/// <summary>
/// A single line item inside an Order.
/// Captures an immutable snapshot of the cart item at the time the order was placed:
/// MenuItemId, Name, Price, Quantity, and Customization. Once the parent order is
/// created these values must not change, even if the underlying menu item is later
/// edited or removed.
/// </summary>
public class OrderItem : BaseEntity
{
    /// <summary>Foreign key to the parent order.</summary>
    public Guid OrderId { get; set; }

    /// <summary>Reference to the MenuItem (from Menu-Service) this line represents.</summary>
    public Guid MenuItemId { get; set; }

    /// <summary>Item name captured at time of order.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Unit price captured at time of order.</summary>
    public decimal Price { get; set; }

    /// <summary>How many units of this item were ordered.</summary>
    public int Quantity { get; set; }

    /// <summary>Optional customisation notes (e.g. "no onions").</summary>
    public string? Customization { get; set; }

    /// <summary>Navigation property to the parent order.</summary>
    public Order? Order { get; set; }
}
