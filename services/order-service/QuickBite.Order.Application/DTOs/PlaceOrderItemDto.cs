namespace QuickBite.Order.Application.DTOs;

/// <summary>
/// A single line item supplied at order placement.
/// Carries the snapshot data taken from the customer's cart.
/// </summary>
public class PlaceOrderItemDto
{
    public Guid MenuItemId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string? Customization { get; set; }
}
