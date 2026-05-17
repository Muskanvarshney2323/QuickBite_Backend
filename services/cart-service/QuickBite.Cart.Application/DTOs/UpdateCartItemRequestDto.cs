// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Cart.Application.DTOs;

// ========================= UPDATE CART ITEM REQUEST DTO =========================
/// <summary>
/// UpdateCartItemRequestDto: Request DTO to change quantity of cart item
/// Used in PUT /api/v1/cart/updateQty/{cartItemId} endpoint
/// Updates existing cart item quantity (for adding/reducing count)
/// </summary>
public class UpdateCartItemRequestDto
{
    // ========================= QUANTITY =========================
    // Quantity: New quantity for this cart item
    // Type: Integer
    // Example: 3 (meaning 3 units of the item)
    // Validation: Must be greater than 0
    // Business Logic: Server recalculates cart total after update
    // Required: Yes
    public int Quantity { get; set; }
}
