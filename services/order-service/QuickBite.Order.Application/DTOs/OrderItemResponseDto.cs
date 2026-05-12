namespace QuickBite.Order.Application.DTOs;

/// <summary>
/// Response DTO for a single line item in an order.
/// </summary>
public class OrderItemResponseDto
{
    public Guid OrderItemId { get; set; }

    public Guid MenuItemId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string? Customization { get; set; }

    public decimal LineTotal { get; set; }
}
