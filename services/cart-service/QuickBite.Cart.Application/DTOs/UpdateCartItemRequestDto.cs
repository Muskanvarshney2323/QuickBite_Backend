namespace QuickBite.Cart.Application.DTOs;

/// <summary>
/// Request DTO used to update quantity of an existing cart item.
/// </summary>
public class UpdateCartItemRequestDto
{
    public int Quantity { get; set; }
}