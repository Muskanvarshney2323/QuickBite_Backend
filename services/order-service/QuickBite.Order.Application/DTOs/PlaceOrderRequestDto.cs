// Import for PaymentMode enum (payment method selection)
using QuickBite.Order.Domain.Enums;

// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Order.Application.DTOs;

// ========================= PLACE ORDER REQUEST DTO =========================
/// <summary>
/// PlaceOrderRequestDto: Request DTO to place a new order
/// Used in POST /api/v1/orders/place endpoint
/// In full deployment OrderService would call Cart-Service for item snapshot
/// Here items are supplied directly for independent operation and unit-testability
/// 
/// Frontend request body format:
/// {
///   "customerId": "guid-string (required)",
///   "restaurantId": "guid-string (required)",
///   "items": [{ "menuItemId": "guid", "name": "string", "price": decimal, "quantity": int, "customization": "string-or-null" }],
///   "discount": decimal (optional, default 0),
///   "modeOfPayment": enum-value (1=CASH_ON_DELIVERY, 2=CARD, 3=UPI, 4=WALLET, optional default 1),
///   "deliveryAddress": "string (required)",
///   "specialInstructions": "string-or-null (optional)",
///   "estimatedMinutes": int-or-null (optional, default 45)
/// }
/// </summary>
public class PlaceOrderRequestDto
{
    // ========================= CUSTOMER ID =========================
    /// <summary>The customer placing the order. Must be a valid GUID.</summary>
    // Type: GUID
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // Identifies which customer is making this order
    // Required: Yes (cannot be empty)
    public Guid CustomerId { get; set; }

    // ========================= RESTAURANT ID =========================
    /// <summary>The restaurant fulfilling the order. Must be a valid GUID.</summary>
    // Type: GUID
    // Example: "660e8400-e29b-41d4-a716-446655440000"
    // Specifies which restaurant will prepare this order
    // Required: Yes (cannot be empty)
    public Guid RestaurantId { get; set; }

    // ========================= ORDER ITEMS =========================
    /// <summary>Snapshot of cart items at the moment of placement. Must not be empty.</summary>
    // Type: List of PlaceOrderItemDto
    // Contains all menu items customer wants to order
    // Each item includes: MenuItemId, Name, Price (snapshot), Quantity, Customization
    // Business Logic: All items must be from same restaurant
    // Validation: List must have at least 1 item
    // Required: Yes (cannot be empty list)
    public List<PlaceOrderItemDto> Items { get; set; } = new();

    // ========================= DISCOUNT =========================
    /// <summary>Discount to apply (e.g. resolved by the promo engine on the cart). Default is 0.</summary>
    // Type: Decimal (currency amount)
    // Example: 50.00 (for ₹50.00 discount)
    // Source: Resolved from promo code applied to cart
    // Default: 0 (no discount)
    // Validation: Must be non-negative
    // Required: No (optional, defaults to 0)
    public decimal Discount { get; set; }

    // ========================= MODE OF PAYMENT =========================
    /// <summary>Payment mode: 1=CASH_ON_DELIVERY, 2=CARD, 3=UPI, 4=WALLET. Default is 1 (CASH_ON_DELIVERY).</summary>
    // Type: PaymentMode enum (1, 2, 3, or 4)
    // Example values:
    //   1 = CASH_ON_DELIVERY (pay when delivery arrives)
    //   2 = CARD (credit/debit card payment)
    //   3 = UPI (United Payments Interface)
    //   4 = WALLET (QuickBite e-wallet balance)
    // Default: CASH_ON_DELIVERY
    // Business Logic: Triggers different payment gateway flows
    // Required: No (optional, defaults to CASH_ON_DELIVERY)
    public PaymentMode ModeOfPayment { get; set; } = PaymentMode.CASH_ON_DELIVERY;

    // ========================= DELIVERY ADDRESS =========================
    /// <summary>Delivery address for this order. Required field, cannot be empty.</summary>
    // Type: String
    // Example: "123 Main Street, Apt 4B, New York, NY 10001"
    // Business Logic: Used to assign delivery agent and compute delivery route
    // Validation: Non-empty string required
    // Required: Yes (cannot be null or empty)
    public string DeliveryAddress { get; set; } = string.Empty;

    // ========================= SPECIAL INSTRUCTIONS =========================
    /// <summary>Special instructions from the customer (e.g. "no onions", "extra sauce"). Optional.</summary>
    // Type: String (nullable)
    // Example: "Ring doorbell twice, leave at side entrance"
    // Source: Customer-provided delivery instructions
    // Business Logic: Sent to restaurant and delivery agent
    // Required: No (optional, can be null or empty)
    public string? SpecialInstructions { get; set; }

    // ========================= ESTIMATED MINUTES =========================
    /// <summary>
    /// Estimated preparation time in minutes. If not supplied, defaults to 45 minutes.
    /// Used to compute EstimatedDelivery as OrderDate + EstimatedMinutes.
    /// </summary>
    // Type: Integer (nullable)
    // Example: 45 (meaning 45 minutes from order placed to delivery)
    // Default: 45 minutes
    // Business Logic: Computed as OrderDate + EstimatedMinutes = EstimatedDelivery
    // Validation: If provided, must be positive number
    // Required: No (optional, defaults to 45)
    public int? EstimatedMinutes { get; set; }
}
