namespace QuickBite.Cart.Application.DTOs;

/// <summary>
/// Response DTO for a single cart item.
/// </summary>
public class CartItemResponseDto
{
    public Guid CartItemId { get; set; }

    public Guid MenuItemId { get; set; }

    public string MenuItemName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }
}