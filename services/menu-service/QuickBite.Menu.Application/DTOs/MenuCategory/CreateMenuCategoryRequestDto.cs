// Import for data validation attributes
using System.ComponentModel.DataAnnotations;

// Namespace for Menu Category related Data Transfer Objects
namespace QuickBite.Menu.Application.DTOs.MenuCategory
{
    // ========================= CREATE MENU CATEGORY REQUEST DTO =========================
    /// <summary>
    /// CreateMenuCategoryRequestDto: Request DTO to create new menu category
    /// Used in POST /api/v1/menu/categories endpoint
    /// Creates category to group related menu items (e.g., "Curries", "Breads")
    /// </summary>
    public class CreateMenuCategoryRequestDto
    {
        // ========================= NAME =========================
        // Name: Category name/identifier
        // Type: String (required, max 100 characters)
        // Example: "Main Course", "Breads", "Desserts"
        // Validation: [Required], [MaxLength(100)]
        // Business Logic: Displayed in menu UI for grouping items
        // Required: Yes (cannot be null or empty)
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // ========================= DESCRIPTION =========================
        // Description: Optional description of what this category contains
        // Type: String (optional, max 300 characters)
        // Example: "All vegetarian curry options"
        // Validation: [MaxLength(300)]
        // Business Logic: Helps customers understand category purpose
        // Required: No (can be null or empty)
        [MaxLength(300)]
        public string? Description { get; set; }

        // ========================= RESTAURANT ID =========================
        // RestaurantId: Restaurant this category belongs to
        // Type: GUID (required)
        // Example: "550e8400-e29b-41d4-a716-446655440000"
        // Validation: [Required]
        // Business Logic: Each restaurant has its own set of categories
        // Relationship: Many categories per restaurant
        // Required: Yes (must be valid restaurant GUID)
        [Required]
        public Guid RestaurantId { get; set; }
    }
}