// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Order.Application.DTOs;

// ========================= ORDER ITEM RESPONSE DTO =========================
/// <summary>
/// OrderItemResponseDto: Response DTO for single line item in order
/// Used in OrderResponseDto.Items collection
/// Contains details of one menu item included in the order
/// </summary>
public class OrderItemResponseDto
{
    // ========================= ORDER ITEM ID =========================
    // OrderItemId: Unique identifier for this order line item
    // Type: GUID
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // Note: Different from MenuItemId; identifies this specific order line
    public Guid OrderItemId { get; set; }

    // ========================= MENU ITEM ID =========================
    // MenuItemId: Reference to original menu item from restaurant menu
    // Type: GUID
    // Example: "660e8400-e29b-41d4-a716-446655440000"
    // Links to MenuItem entity in Menu Service
    public Guid MenuItemId { get; set; }

    // ========================= ITEM NAME =========================
    // Name: Human-readable name of menu item
    // Type: String
    // Example: "Butter Chicken"
    // Snapshot: Captured from menu at order placement time
    // Used for display in order receipt and order tracking UI
    public string Name { get; set; } = string.Empty;

    // ========================= UNIT PRICE =========================
    // Price: Price per unit (snapshot from time of order placement)
    // Type: Decimal (currency amount)
    // Example: 450.50 (for ₹450.50)
    // Important: This is historical snapshot price, not current menu price
    // Business Logic: Protects against menu price changes after order placed
    public decimal Price { get; set; }

    // ========================= QUANTITY =========================
    // Quantity: Number of units of this item in order
    // Type: Integer
    // Example: 2 (meaning 2 units)
    // Used to calculate line total (Price × Quantity)
    public int Quantity { get; set; }

    // ========================= CUSTOMIZATION =========================
    // Customization: Optional special preparation instructions for this item
    // Type: String (nullable)
    // Example: "No onions, extra sauce, extra spicy"
    // Business Logic: Sent to restaurant kitchen
    // Can be null if no special customization requested
    public string? Customization { get; set; }

    // ========================= LINE TOTAL =========================
    // LineTotal: Total cost for this specific line item
    // Type: Decimal (currency amount)
    // Example: 901.00 (for ₹901.00)
    // Calculation: Price × Quantity = 450.50 × 2 = 901.00
    // Used for line-level display and order total calculation
    public decimal LineTotal { get; set; }
}
