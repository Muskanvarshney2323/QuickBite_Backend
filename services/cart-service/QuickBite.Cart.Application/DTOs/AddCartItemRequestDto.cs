// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Cart.Application.DTOs;

// ========================= ADD CART ITEM REQUEST DTO =========================
/// <summary>
/// AddCartItemRequestDto: Request DTO for adding item to cart
/// Used in POST /api/v1/cart/addItem endpoint
/// Contains all data needed to add menu item with price snapshot
/// </summary>
public class AddCartItemRequestDto
{
    // ========================= CUSTOMER ID =========================
    // CustomerId: Unique identifier of customer adding item
    // Type: GUID (Globally Unique Identifier)
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // Required: Yes
    public Guid CustomerId { get; set; }

    // ========================= RESTAURANT ID =========================
    // RestaurantId: Restaurant this item belongs to
    // Type: GUID
    // Example: "660e8400-e29b-41d4-a716-446655440000"
    // Business Rule: All items in cart must be from same restaurant
    // Required: Yes
    public Guid RestaurantId { get; set; }

    // ========================= MENU ITEM ID =========================
    // MenuItemId: Unique identifier of menu item being added
    // Type: GUID
    // Example: "770e8400-e29b-41d4-a716-446655440000"
    // Required: Yes
    public Guid MenuItemId { get; set; }

    // ========================= ITEM NAME =========================
    // Name: Item name from menu (for display/reference)
    // Type: String
    // Example: "Butter Chicken"
    // Required: Yes (should not be empty)
    public string Name { get; set; } = string.Empty;

    // ========================= PRICE SNAPSHOT =========================
    /// <summary>
    /// Price snapshot captured at moment of add
    /// Protects against menu price changes after item is added
    /// Server uses this price, not current menu price
    /// </summary>
    // Type: Decimal (currency amount)
    // Example: 450.50 (for ₹450.50)
    // Required: Yes
    public decimal Price { get; set; }

    // ========================= QUANTITY =========================
    // Quantity: Number of units of this item to add
    // Type: Integer
    // Example: 2 (meaning 2 units)
    // Validation: Must be greater than 0
    // Required: Yes
    public int Quantity { get; set; }

    // ========================= CUSTOMIZATION =========================
    /// <summary>
    /// Optional customization notes (e.g. "no onions", "extra spicy")
    /// Special instructions for preparing this item
    /// </summary>
    // Type: String (nullable - can be null or empty)
    // Example: "No onions, extra sauce"
    // Required: No (can be null)
    public string? Customization { get; set; }
}
