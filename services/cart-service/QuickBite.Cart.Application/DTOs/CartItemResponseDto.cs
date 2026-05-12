namespace QuickBite.Cart.Application.DTOs;

/// <summary>
/// Response DTO for a single cart item.
/// </summary>
public class CartItemResponseDto
{
    public Guid ItemId { get; set; }

    public Guid MenuItemId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string? Customization { get; set; }

    /// <summary>
    /// Convenience total for this line (Price * Quantity).
    /// </summary>
    public decimal LineTotal { get; set; }
}
