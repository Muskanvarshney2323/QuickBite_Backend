using QuickBite.Order.Domain.Enums;

namespace QuickBite.Order.Application.DTOs;

/// <summary>
/// Request DTO used to place a new order.
/// In a fully wired-up deployment the OrderService would call Cart-Service to
/// snapshot the items; here the snapshot is supplied directly so the service
/// can run independently and remain unit-testable.
/// </summary>
public class PlaceOrderRequestDto
{
    public Guid CustomerId { get; set; }

    public Guid RestaurantId { get; set; }

    /// <summary>Snapshot of cart items at the moment of placement.</summary>
    public List<PlaceOrderItemDto> Items { get; set; } = new();

    /// <summary>Discount to apply (e.g. resolved by the promo engine on the cart).</summary>
    public decimal Discount { get; set; }

    public PaymentMode ModeOfPayment { get; set; } = PaymentMode.CASH_ON_DELIVERY;

    public string DeliveryAddress { get; set; } = string.Empty;

    public string? SpecialInstructions { get; set; }

    /// <summary>
    /// How many minutes the order is expected to take.
    /// Used to compute EstimatedDelivery; defaults to 45 if not supplied.
    /// </summary>
    public int? EstimatedMinutes { get; set; }
}
