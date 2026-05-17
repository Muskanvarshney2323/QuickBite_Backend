// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Review.Application.DTOs;

// ========================= ADD REVIEW REQUEST DTO =========================
/// <summary>
/// AddReviewRequestDto: Request DTO when customer submits review for completed order
/// Used in POST /api/v1/reviews endpoint
/// Captures food quality and delivery ratings plus optional comment
/// </summary>
public class AddReviewRequestDto
{
    // ========================= ORDER ID =========================
    // OrderId: Order being reviewed
    // Type: GUID
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // Business Logic: One review per order maximum
    // Required: Yes
    public Guid OrderId { get; set; }

    // ========================= CUSTOMER ID =========================
    // CustomerId: Customer submitting the review
    // Type: GUID
    // Example: "660e8400-e29b-41d4-a716-446655440000"
    // Used to verify customer ordered this food
    // Required: Yes
    public Guid CustomerId { get; set; }

    // ========================= RESTAURANT ID =========================
    // RestaurantId: Restaurant being reviewed
    // Type: GUID
    // Example: "770e8400-e29b-41d4-a716-446655440000"
    // Links review to restaurant for rating aggregation
    // Required: Yes
    public Guid RestaurantId { get; set; }

    // ========================= AGENT ID =========================
    // AgentId: Delivery agent who delivered this order
    // Type: GUID
    // Example: "880e8400-e29b-41d4-a716-446655440000"
    // Allows separate rating of delivery partner
    // Required: Yes
    public Guid AgentId { get; set; }

    // ========================= FOOD RATING =========================
    /// <summary>Food rating (1 to 5).</summary>
    // Type: Integer (1-5)
    // Example: 5 (excellent food), 1 (poor quality)
    // Scale: 1=Poor, 2=Fair, 3=Good, 4=Very Good, 5=Excellent
    // Used to: Calculate restaurant average food rating
    // Required: Yes (validation needed: 1 <= rating <= 5)
    public int FoodRating { get; set; }

    // ========================= DELIVERY RATING =========================
    /// <summary>Delivery rating (1 to 5).</summary>
    // Type: Integer (1-5)
    // Example: 4 (on time, professional), 2 (late, careless handling)
    // Scale: 1=Poor, 2=Fair, 3=Good, 4=Very Good, 5=Excellent
    // Used to: Calculate delivery agent average rating
    // Required: Yes (validation needed: 1 <= rating <= 5)
    public int DeliveryRating { get; set; }

    // ========================= COMMENT =========================
    /// <summary>Optional comment.</summary>
    // Type: String (optional)
    // Example: "Great taste! Could be hotter. Packaging was good."
    // Business Logic: Shown publicly on restaurant profile
    // Max length: Usually 500-1000 characters (validate on server)
    // Required: No (can be null or empty)
    public string Comment { get; set; } = string.Empty;
}
