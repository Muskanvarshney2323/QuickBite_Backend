// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Cart.Application.DTOs;

// ========================= CART ITEM RESPONSE DTO =========================
/// <summary>
/// CartItemResponseDto: Response DTO for single item in cart
/// Used in CartResponseDto.Items collection
/// Contains all details of one line item in the shopping cart
/// </summary>
public class CartItemResponseDto
{
    // ========================= ITEM ID =========================
    // ItemId: Unique identifier of this cart item line
    // Type: GUID
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // Note: Different from MenuItemId; identifies specific cart line
    public Guid ItemId { get; set; }

    // ========================= MENU ITEM ID =========================
    // MenuItemId: Reference to menu item from restaurant menu
    // Type: GUID
    // Example: "660e8400-e29b-41d4-a716-446655440000"
    // Links to MenuItem entity in Menu Service
    public Guid MenuItemId { get; set; }

    // ========================= ITEM NAME =========================
    // Name: Human-readable name of item
    // Type: String
    // Example: "Butter Chicken"
    // Used for display in shopping cart UI
    public string Name { get; set; } = string.Empty;

    // ========================= UNIT PRICE =========================
    // Price: Price per unit (snapshot from time of add)
    // Type: Decimal (currency amount)
    // Example: 450.50 (for ₹450.50)
    // Note: This is snapshot price, not current menu price
    public decimal Price { get; set; }

    // ========================= QUANTITY =========================
    // Quantity: Number of units of this item in cart
    // Type: Integer
    // Example: 2 (meaning 2 units)
    // Used to calculate line total (Price × Quantity)
    public int Quantity { get; set; }

    // ========================= CUSTOMIZATION =========================
    // Customization: Optional special instructions for this item
    // Type: String (nullable)
    // Example: "No onions, extra sauce"
    // Can be null if no customization
    public string? Customization { get; set; }

    // ========================= LINE TOTAL =========================
    /// <summary>
    /// Convenience total for this specific line item
    /// Calculated as: Price × Quantity
    /// Example: 450.50 × 2 = 901.00
    /// </summary>
    // Type: Decimal (currency amount)
    // Example: 901.00 (for ₹901.00)
    // Used for line-level display and cart total calculation
    public decimal LineTotal { get; set; }
}
