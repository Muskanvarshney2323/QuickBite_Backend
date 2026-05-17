// Namespace for Menu Item related Data Transfer Objects
namespace QuickBite.Menu.Application.DTOs.MenuItem
{
    // ========================= MENU ITEM RESPONSE DTO =========================
    /// <summary>
    /// MenuItemResponseDto: Response DTO for menu item details
    /// Used in GET /api/v1/menu/items/{itemId} and list endpoints
    /// Returns complete menu item data including category information
    /// </summary>
    public class MenuItemResponseDto
    {
        // ========================= ID =========================
        // Id: Unique identifier for this menu item
        // Type: GUID
        // Example: "550e8400-e29b-41d4-a716-446655440000"
        // Database primary key
        public Guid Id { get; set; }

        // ========================= NAME =========================
        // Name: Display name of the menu item
        // Type: String
        // Example: "Butter Chicken"
        // Shown in menu UI and restaurant catalog
        public string Name { get; set; } = string.Empty;

        // ========================= DESCRIPTION =========================
        // Description: Detailed explanation of item
        // Type: String (nullable)
        // Example: "Tender chicken in creamy tomato sauce"
        // Helps customers understand what they're ordering
        public string? Description { get; set; }

        // ========================= PRICE =========================
        // Price: Current selling price of this item
        // Type: Decimal (currency amount)
        // Example: 450.50 (for ₹450.50)
        // Used in cart and order calculations
        public decimal Price { get; set; }

        // ========================= IS AVAILABLE =========================
        // IsAvailable: Whether item is currently available
        // Type: Boolean
        // Example: true (in stock), false (out of stock)
        // Determines if item can be ordered
        public bool IsAvailable { get; set; }

        // ========================= IMAGE URL =========================
        // ImageUrl: URL to item's display image
        // Type: String (nullable)
        // Example: "https://cdn.quickbite.com/images/butter-chicken.jpg"
        // Shown in menu UI for visual appeal
        public string? ImageUrl { get; set; }

        // ========================= MENU CATEGORY ID =========================
        // MenuCategoryId: Category this item belongs to
        // Type: GUID
        // Example: "770e8400-e29b-41d4-a716-446655440000"
        // Links to MenuCategory entity
        public Guid MenuCategoryId { get; set; }

        // ========================= RESTAURANT ID =========================
        // RestaurantId: Restaurant that owns this menu item
        // Type: GUID
        // Example: "880e8400-e29b-41d4-a716-446655440000"
        // These fields help frontend filter and display menu items
        // Used for multi-restaurant menu filtering
        public Guid RestaurantId { get; set; }

        // ========================= CATEGORY =========================
        // Category: Category identifier/code
        // Type: String
        // Example: "CURRIES", "BREADS", "DESSERTS"
        // These fields help the frontend filter and display menu items correctly
        // Used for UI categorization
        public string Category { get; set; } = string.Empty;

        // ========================= CATEGORY NAME =========================
        // CategoryName: Human-readable category name
        // Type: String
        // Example: "Main Course", "Bread", "Desserts"
        // These fields help the frontend filter and display menu items correctly
        // Displayed in UI to help customers browse
        public string CategoryName { get; set; } = string.Empty;
    }
}
