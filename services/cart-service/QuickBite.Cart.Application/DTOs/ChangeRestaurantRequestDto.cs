// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Cart.Application.DTOs;

// ========================= CHANGE RESTAURANT REQUEST DTO =========================
/// <summary>
/// ChangeRestaurantRequestDto: Request DTO to switch cart to different restaurant
/// Used in POST /api/v1/cart/changeRestaurant endpoint
/// Clears all items because single-restaurant ordering is enforced
/// </summary>
public class ChangeRestaurantRequestDto
{
    // ========================= CUSTOMER ID =========================
    // CustomerId: Unique identifier of customer changing restaurant
    // Type: GUID
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // Required: Yes (must not be empty)
    public Guid CustomerId { get; set; }

    // ========================= NEW RESTAURANT ID =========================
    // NewRestaurantId: Restaurant ID to switch cart to
    // Type: GUID
    // Example: "660e8400-e29b-41d4-a716-446655440000"
    // Business Rule: All existing items will be cleared
    // Business Rule: Cart will be bound to this new restaurant
    // Required: Yes (must not be empty)
    public Guid NewRestaurantId { get; set; }
}
