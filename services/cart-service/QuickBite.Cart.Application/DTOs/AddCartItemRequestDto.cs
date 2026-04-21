namespace QuickBite.Cart.Application.DTOs;

/// <summary>
/// Request DTO used to add a new item to the cart.
/// </summary>
public class AddCartItemRequestDto
{
    public Guid UserId { get; set; }

    public Guid MenuItemId { get; set; }

    public string MenuItemName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }
}