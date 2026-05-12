namespace QuickBite.Cart.Application.DTOs;

/// <summary>
/// Response DTO for full cart details.
/// </summary>
public class CartResponseDto
{
    public Guid CartId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid RestaurantId { get; set; }

    public List<CartItemResponseDto> Items { get; set; } = new();

    /// <summary>
    /// Total price of the cart (sum of all line totals).
    /// </summary>
    public decimal TotalPrice { get; set; }
}
