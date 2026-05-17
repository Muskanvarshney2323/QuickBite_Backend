// Import for order and payment enums
using QuickBite.Order.Domain.Enums;

// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Order.Application.DTOs;

// ========================= ORDER RESPONSE DTO =========================
/// <summary>
/// OrderResponseDto: Response DTO for complete order details
/// Used in response to /api/v1/orders endpoints
/// Contains order metadata, amounts, items, and delivery information
/// </summary>
public class OrderResponseDto
{
    // ========================= ORDER ID =========================
    // OrderId: Unique identifier for this order
    // Type: GUID
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // Identifies specific order in database
    public Guid OrderId { get; set; }

    // ========================= CUSTOMER ID =========================
    // CustomerId: Customer who placed this order
    // Type: GUID
    // Example: "660e8400-e29b-41d4-a716-446655440000"
    // Links order to customer record
    public Guid CustomerId { get; set; }

    // ========================= RESTAURANT ID =========================
    // RestaurantId: Restaurant fulfilling this order
    // Type: GUID
    // Example: "770e8400-e29b-41d4-a716-446655440000"
    // Identifies which restaurant is preparing order
    public Guid RestaurantId { get; set; }

    // ========================= DELIVERY AGENT ID =========================
    // DeliveryAgentId: Delivery personnel assigned to this order
    // Type: GUID (nullable)
    // Example: "880e8400-e29b-41d4-a716-446655440000"
    // Can be null if not yet assigned to delivery partner
    public Guid? DeliveryAgentId { get; set; }

    // ========================= TOTAL AMOUNT =========================
    // TotalAmount: Sum of all line totals before discount
    // Type: Decimal (currency amount)
    // Example: 950.00 (for ₹950.00)
    // Calculated as: SUM(Price × Quantity for each item)
    // Business Logic: Does not include discount
    public decimal TotalAmount { get; set; }

    // ========================= DISCOUNT =========================
    // Discount: Discount amount applied to order
    // Type: Decimal (currency amount)
    // Example: 50.00 (for ₹50.00 discount)
    // Source: From promo code or promotional offer
    // Business Logic: Subtracted from TotalAmount
    public decimal Discount { get; set; }

    // ========================= FINAL AMOUNT =========================
    // FinalAmount: Amount customer actually pays
    // Type: Decimal (currency amount)
    // Example: 900.00 (for ₹900.00)
    // Calculation: TotalAmount - Discount
    // Business Logic: This is the amount sent to payment gateway
    public decimal FinalAmount { get; set; }

    // ========================= MODE OF PAYMENT =========================
    // ModeOfPayment: Payment method for this order
    // Type: PaymentMode enum (1, 2, 3, or 4)
    // Example values:
    //   1 = CASH_ON_DELIVERY
    //   2 = CARD
    //   3 = UPI
    //   4 = WALLET
    // Business Logic: Determines payment processing flow
    public PaymentMode ModeOfPayment { get; set; }

    // ========================= ORDER STATUS =========================
    // OrderStatus: Current status of this order
    // Type: OrderStatus enum
    // Example values: PENDING, CONFIRMED, PREPARING, READY, PICKED_UP, DELIVERED, CANCELLED
    // Business Logic: Updated throughout order lifecycle
    public OrderStatus OrderStatus { get; set; }

    // ========================= ORDER DATE =========================
    // OrderDate: Timestamp when order was placed
    // Type: DateTime
    // Example: "2026-05-17T14:30:00Z"
    // Set when order is created
    // Business Logic: Used to track order age and SLA compliance
    public DateTime OrderDate { get; set; }

    // ========================= ESTIMATED DELIVERY =========================
    // EstimatedDelivery: Predicted delivery timestamp
    // Type: DateTime (nullable)
    // Example: "2026-05-17T15:15:00Z" (45 minutes after OrderDate)
    // Calculation: OrderDate + EstimatedMinutes
    // Can be null if order is cancelled
    public DateTime? EstimatedDelivery { get; set; }

    // ========================= DELIVERY ADDRESS =========================
    // DeliveryAddress: Location where order should be delivered
    // Type: String
    // Example: "123 Main Street, Apt 4B, New York, NY 10001"
    // Business Logic: Provided to delivery agent for navigation
    public string DeliveryAddress { get; set; } = string.Empty;

    // ========================= SPECIAL INSTRUCTIONS =========================
    // SpecialInstructions: Additional notes from customer
    // Type: String (nullable)
    // Example: "Ring doorbell twice, leave at side entrance"
    // Business Logic: Shown to restaurant staff and delivery agent
    public string? SpecialInstructions { get; set; }

    // ========================= ORDER ITEMS =========================
    // Items: List of all items in this order
    // Type: List of OrderItemResponseDto
    // Each item contains: OrderItemId, MenuItemId, Name, Price, Quantity, Customization, LineTotal
    // Business Logic: Reflects snapshot of items at order placement time
    public List<OrderItemResponseDto> Items { get; set; } = new();
}
