// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.Review.Application.DTOs;

// ========================= REVIEW RESPONSE DTO =========================
/// <summary>
/// ReviewResponseDto: Response DTO for review record details
/// Used in response to /api/v1/reviews endpoints
/// Contains complete review including ratings, comment, and verification status
/// </summary>
public class ReviewResponseDto
{
    // ========================= REVIEW ID =========================
    // ReviewId: Unique identifier for this review
    // Type: GUID
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // Database primary key
    public Guid ReviewId { get; set; }

    // ========================= ORDER ID =========================
    // OrderId: Order being reviewed
    // Type: GUID
    // Example: "660e8400-e29b-41d4-a716-446655440000"
    // Links review to specific order
    public Guid OrderId { get; set; }

    // ========================= CUSTOMER ID =========================
    // CustomerId: Customer who submitted review
    // Type: GUID
    // Example: "770e8400-e29b-41d4-a716-446655440000"
    // Shows who wrote this review
    public Guid CustomerId { get; set; }

    // ========================= RESTAURANT ID =========================
    // RestaurantId: Restaurant being reviewed
    // Type: GUID
    // Example: "880e8400-e29b-41d4-a716-446655440000"
    // Groups reviews by restaurant
    public Guid RestaurantId { get; set; }

    // ========================= AGENT ID =========================
    // AgentId: Delivery agent being reviewed
    // Type: GUID
    // Example: "990e8400-e29b-41d4-a716-446655440000"
    // Tracks delivery partner ratings
    public Guid AgentId { get; set; }

    // ========================= FOOD RATING =========================
    // FoodRating: Food quality rating (1-5)
    // Type: Integer (1-5)
    // Example: 5 (excellent), 3 (average), 1 (poor)
    // Used to calculate restaurant food rating
    public int FoodRating { get; set; }

    // ========================= DELIVERY RATING =========================
    // DeliveryRating: Delivery service rating (1-5)
    // Type: Integer (1-5)
    // Example: 4 (on time and professional), 2 (late)
    // Used to calculate delivery agent rating
    public int DeliveryRating { get; set; }

    // ========================= COMMENT =========================
    // Comment: Customer's written feedback
    // Type: String
    // Example: "Great taste but could be hotter. Excellent packaging!"
    // Shown publicly on restaurant profile
    // Can be empty if no comment provided
    public string Comment { get; set; } = string.Empty;

    // ========================= REVIEW DATE =========================
    // ReviewDate: Timestamp when review was submitted
    // Type: DateTime
    // Example: "2026-05-17T15:30:00Z"
    // Used for sorting reviews (newest first)
    public DateTime ReviewDate { get; set; }

    // ========================= IS VERIFIED =========================
    // IsVerified: Whether this review is from verified customer purchase
    // Type: Boolean
    // Example: true (customer actually placed this order)
    // Business Logic: Verified reviews shown first
    // Helps prevent fake reviews
    public bool IsVerified { get; set; }
}
