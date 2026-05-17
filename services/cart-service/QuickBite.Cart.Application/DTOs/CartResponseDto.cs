// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Cart.Application.DTOs;

// ========================= CART RESPONSE DTO =========================
/// <summary>
/// CartResponseDto: Response DTO for complete cart details
/// Used in response to /api/v1/cart endpoints
/// Contains cart ID, customer, restaurant, all items, and total price
/// </summary>
public class CartResponseDto
{
    // ========================= CART ID =========================
    // CartId: Unique identifier for this cart
    // Type: GUID
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // Identifies specific cart in database
    public Guid CartId { get; set; }

    // ========================= CUSTOMER ID =========================
    // CustomerId: Owner of this cart
    // Type: GUID
    // Example: "660e8400-e29b-41d4-a716-446655440000"
    // Links cart to customer
    public Guid CustomerId { get; set; }

    // ========================= RESTAURANT ID =========================
    // RestaurantId: Restaurant all items are from
    // Type: GUID
    // Example: "770e8400-e29b-41d4-a716-446655440000"
    // Business Rule: Single-restaurant ordering enforced
    public Guid RestaurantId { get; set; }

    // ========================= CART ITEMS =========================
    // Items: List of all items in this cart
    // Type: List of CartItemResponseDto
    // Each item contains: MenuItemId, Name, Price, Quantity, Customization, LineTotal
    // Count: Can be 0 if cart is empty
    public List<CartItemResponseDto> Items { get; set; } = new();

    // ========================= TOTAL PRICE =========================
    /// <summary>
    /// Total price of entire cart (sum of all line totals)
    /// Calculated as: SUM(Price × Quantity for each item)
    /// Updated whenever items added/removed/quantity changed
    /// </summary>
    // Type: Decimal (currency amount)
    // Example: 900.50 (for ₹900.50)
    // Server-side computed (prevents client manipulation)
    public decimal TotalPrice { get; set; }
}
