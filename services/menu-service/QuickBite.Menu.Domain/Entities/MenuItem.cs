namespace QuickBite.Menu.Domain.Entities
{
    // This entity represents a food item inside a category
    public class MenuItem
    {
        // Primary key for the MenuItem table
        public Guid Id { get; set; }

        // Name of the food item
        public string Name { get; set; } = string.Empty;

        // Optional description of the menu item
        public string? Description { get; set; }

        // Price of the item
        public decimal Price { get; set; }

        // Indicates whether item is available or not
        public bool IsAvailable { get; set; } = true;

        // Optional image URL for menu item
        public string? ImageUrl { get; set; }

        // Foreign key for MenuCategory
        public Guid MenuCategoryId { get; set; }

        // Navigation property to access parent category
        public MenuCategory? MenuCategory { get; set; }
    }
}