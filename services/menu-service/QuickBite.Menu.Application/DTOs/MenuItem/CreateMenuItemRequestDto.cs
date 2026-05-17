// Import for data validation attributes
using System.ComponentModel.DataAnnotations;

// Namespace for Menu Item related Data Transfer Objects
namespace QuickBite.Menu.Application.DTOs.MenuItem
{
    // ========================= CREATE MENU ITEM REQUEST DTO =========================
    /// <summary>
    /// CreateMenuItemRequestDto: Request DTO to add new menu item to restaurant
    /// Used in POST /api/v1/menu/items endpoint
    /// Creates new menu item in specific restaurant's menu with validation
    /// </summary>
    public class CreateMenuItemRequestDto
    {
        // ========================= NAME =========================
        // Name: Display name of the menu item
        // Type: String (required, max 100 characters)
        // Example: "Butter Chicken", "Paneer Tikka"
        // Validation: [Required], [MaxLength(100)]
        // Business Logic: Displayed in restaurant menu and cart
        // Required: Yes (cannot be null or empty)
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // ========================= DESCRIPTION =========================
        // Description: Detailed explanation of what this item is
        // Type: String (optional, max 500 characters)
        // Example: "Tender chicken pieces cooked in creamy tomato-based sauce"
        // Validation: [MaxLength(500)]
        // Business Logic: Helps customers understand and choose items
        // Required: No (can be null or empty)
        [MaxLength(500)]
        public string? Description { get; set; }

        // ========================= PRICE =========================
        // Price: Selling price of this menu item
        // Type: Decimal (currency amount)
        // Example: 450.50 (for ₹450.50)
        // Validation: [Range(0.01, 100000)]
        // Business Logic: Used in cart and order calculations
        // Required: Yes (must be between 0.01 and 100000)
        [Range(0.01, 100000)]
        public decimal Price { get; set; }

        // ========================= IS AVAILABLE =========================
        // IsAvailable: Whether this item is currently available for ordering
        // Type: Boolean
        // Example: true (item is available), false (out of stock)
        // Default: true (available)
        // Business Logic: Determines if item shows in menu and can be ordered
        // Required: No (optional, defaults to true)
        public bool IsAvailable { get; set; } = true;

        // ========================= IMAGE URL =========================
        // ImageUrl: URL link to item's display image
        // Type: String (optional)
        // Example: "https://cdn.quickbite.com/images/butter-chicken.jpg"
        // Business Logic: Shows item photo in menu UI
        // Required: No (can be null or empty)
        public string? ImageUrl { get; set; }

        // ========================= MENU CATEGORY ID =========================
        // MenuCategoryId: Category this item belongs to
        // Type: GUID (required)
        // Example: "770e8400-e29b-41d4-a716-446655440000"
        // Validation: [Required]
        // Business Logic: Links item to category (e.g., "Curries", "Breads", "Desserts")
        // Relationship: Many items can belong to one category
        // Required: Yes (must be valid category GUID)
        [Required]
        public Guid MenuCategoryId { get; set; }
    }
}