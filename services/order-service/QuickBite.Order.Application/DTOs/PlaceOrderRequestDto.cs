using QuickBite.Order.Domain.Enums;

namespace QuickBite.Order.Application.DTOs;

/// <summary>
/// Request DTO used to place a new order.
/// In a fully wired-up deployment the OrderService would call Cart-Service to
/// snapshot the items; here the snapshot is supplied directly so the service
/// can run independently and remain unit-testable.
///
/// Frontend should send:
/// {
///   "customerId": "guid-string",
///   "restaurantId": "guid-string",
///   "items": [{ "menuItemId": "guid-string", "name": "string", "price": decimal, "quantity": int, "customization": "string-or-null" }],
///   "discount": decimal (optional, default 0),
///   "modeOfPayment": enum-value (1=CASH_ON_DELIVERY, 2=CARD, 3=UPI, 4=WALLET, optional default 1),
///   "deliveryAddress": "string (required)",
///   "specialInstructions": "string-or-null",
///   "estimatedMinutes": int-or-null (optional, default 45)
/// }
/// </summary>
public class PlaceOrderRequestDto
{
    /// <summary>The customer placing the order. Must be a valid GUID.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>The restaurant fulfilling the order. Must be a valid GUID.</summary>
    public Guid RestaurantId { get; set; }

    /// <summary>Snapshot of cart items at the moment of placement. Must not be empty.</summary>
    public List<PlaceOrderItemDto> Items { get; set; } = new();

    /// <summary>Discount to apply (e.g. resolved by the promo engine on the cart). Default is 0.</summary>
    public decimal Discount { get; set; }

    /// <summary>Payment mode: 1=CASH_ON_DELIVERY, 2=CARD, 3=UPI, 4=WALLET. Default is 1 (CASH_ON_DELIVERY).</summary>
    public PaymentMode ModeOfPayment { get; set; } = PaymentMode.CASH_ON_DELIVERY;

    /// <summary>Delivery address for this order. Required field, cannot be empty.</summary>
    public string DeliveryAddress { get; set; } = string.Empty;

    /// <summary>Special instructions from the customer (e.g. "no onions", "extra sauce"). Optional.</summary>
    public string? SpecialInstructions { get; set; }

    /// <summary>
    /// Estimated preparation time in minutes. If not supplied, defaults to 45 minutes.
    /// Used to compute EstimatedDelivery as OrderDate + EstimatedMinutes.
    /// </summary>
    public int? EstimatedMinutes { get; set; }
}
