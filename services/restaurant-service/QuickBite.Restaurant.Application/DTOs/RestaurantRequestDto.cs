// Namespace for Restaurant related Data Transfer Objects
namespace QuickBite.Restaurant.Application.DTOs
{
    // ========================= RESTAURANT RESPONSE DTO =========================
    /// <summary>
    /// RestaurantResponseDto: Response DTO for restaurant details
    /// Used in GET /api/v1/restaurants/{id} and list endpoints
    /// Returns complete restaurant profile and operational status
    /// </summary>
    public class RestaurantResponseDto
    {
        // ========================= ID =========================
        // Id: Unique identifier for this restaurant
        // Type: GUID
        // Example: "550e8400-e29b-41d4-a716-446655440000"
        // Database primary key
        public Guid Id { get; set; }

        // ========================= NAME =========================
        // Name: Restaurant brand name
        // Type: String
        // Example: "The Spice Kitchen", "Mumbai Masala"
        // Shown in search results and restaurant profile
        public string Name { get; set; } = string.Empty;

        // ========================= DESCRIPTION =========================
        // Description: Detailed information about restaurant
        // Type: String
        // Example: "Award-winning North Indian cuisine restaurant. Family-owned since 2005."
        // Shown on restaurant profile page
        public string Description { get; set; } = string.Empty;

        // ========================= CUISINE =========================
        // Cuisine: Type of cuisine served
        // Type: String
        // Example: "North Indian", "Chinese", "Italian", "Continental"
        // Used for filtering in search
        public string Cuisine { get; set; } = string.Empty;

        // ========================= ADDRESS =========================
        // Address: Physical location of restaurant
        // Type: String
        // Example: "123 Main Street, Near Metro Station"
        // Used for order delivery and location-based search
        public string Address { get; set; } = string.Empty;

        // ========================= CITY =========================
        // City: City where restaurant is located
        // Type: String
        // Example: "Mumbai", "New York", "bangalore"
        // Used for city-level filtering
        public string City { get; set; } = string.Empty;

        // ========================= PHONE =========================
        // Phone: Contact number for restaurant
        // Type: String
        // Example: "+91-22-1234-5678"
        // Used for support communication
        public string Phone { get; set; } = string.Empty;

        // ========================= OWNER ID =========================
        // OwnerId: User ID of restaurant owner
        // Type: GUID
        // Example: "660e8400-e29b-41d4-a716-446655440000"
        // Links to owner's account in Auth Service
        public Guid OwnerId { get; set; }

        // ========================= IS APPROVED =========================
        // IsApproved: Whether restaurant is approved by platform
        // Type: Boolean
        // Example: true (approved, can take orders), false (pending approval)
        // Business Logic: Only approved restaurants shown to customers
        // Approval required for: Verification of documents, compliance check
        public bool IsApproved { get; set; }

        // ========================= IS OPEN =========================
        // IsOpen: Whether restaurant is currently open for orders
        // Type: Boolean
        // Example: true (accepting orders now), false (closed/offline)
        // Business Logic: Toggled by restaurant owner based on operating hours
        // When false: Orders cannot be placed
        public bool IsOpen { get; set; }

        // ========================= MINIMUM ORDER AMOUNT =========================
        // MinimumOrderAmount: Minimum cart value to place order
        // Type: Decimal (currency amount)
        // Example: 250.00 (for ₹250.00)
        // Business Logic: Orders below this amount are rejected
        // Used for: Validation before order confirmation
        public decimal MinimumOrderAmount { get; set; }

        // ========================= ESTIMATED DELIVERY TIME =========================
        // EstimatedDeliveryTimeInMinutes: Average delivery time from this restaurant
        // Type: Integer (minutes)
        // Example: 45 (45 minutes average delivery time)
        // Shown to customers when viewing restaurant
        // Calculated as: OrderDate + EstimatedDeliveryTimeInMinutes
        public int EstimatedDeliveryTimeInMinutes { get; set; }

        // ========================= AVERAGE RATING =========================
        // AverageRating: Average food quality rating from reviews
        // Type: Double (1.0 to 5.0)
        // Example: 4.5 (excellent reviews)
        // Calculated from: All verified reviews' FoodRating average
        // Used for: Sorting in search results
        public double AverageRating { get; set; }
    }
}