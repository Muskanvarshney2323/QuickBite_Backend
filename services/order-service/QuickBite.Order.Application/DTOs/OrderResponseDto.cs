using QuickBite.Order.Domain.Enums;

namespace QuickBite.Order.Application.DTOs;

/// <summary>
/// Response DTO for full order details.
/// </summary>
public class OrderResponseDto
{
    public Guid OrderId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid RestaurantId { get; set; }

    public Guid? DeliveryAgentId { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal Discount { get; set; }

    public decimal FinalAmount { get; set; }

    public PaymentMode ModeOfPayment { get; set; }

    public OrderStatus OrderStatus { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime? EstimatedDelivery { get; set; }

    public string DeliveryAddress { get; set; } = string.Empty;

    public string? SpecialInstructions { get; set; }

    public List<OrderItemResponseDto> Items { get; set; } = new();
}
