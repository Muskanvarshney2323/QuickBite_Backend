namespace QuickBite.Cart.Application.DTOs;

/// <summary>
/// Response DTO for complete cart details.
/// </summary>
public class CartResponseDto
{
    public Guid CartId { get; set; }

    public Guid UserId { get; set; }

    public List<CartItemResponseDto> Items { get; set; } = new();

    public decimal GrandTotal { get; set; }
}