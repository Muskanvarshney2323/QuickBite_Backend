// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Cart.Application.DTOs;

// ========================= APPLY PROMO CODE REQUEST DTO =========================
/// <summary>
/// ApplyPromoCodeRequestDto: Request DTO to apply promotional code to cart
/// Used in POST /api/v1/cart/applyPromo endpoint
/// Applies discount code and recalculates cart total
/// </summary>
public class ApplyPromoCodeRequestDto
{
    // ========================= CUSTOMER ID =========================
    // CustomerId: Unique identifier of customer applying promo
    // Type: GUID
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // Used to identify which cart to apply discount to
    // Required: Yes
    public Guid CustomerId { get; set; }

    // ========================= PROMO CODE =========================
    // PromoCode: Promotional/discount code entered by customer
    // Type: String
    // Example: "SAVE10" or "WELCOME20"
    // Validation: Non-empty string required
    // Business Logic: Validated against promo service (stub in demo)
    // Required: Yes
    public string PromoCode { get; set; } = string.Empty;
}
