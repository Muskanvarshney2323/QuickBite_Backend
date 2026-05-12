namespace QuickBite.Menu.Domain.Entities
{
    // This entity represents a menu category like Pizza, Burger, Drinks, etc.
    public class MenuCategory
    {
        // Primary key for the MenuCategory table
        public Guid Id { get; set; }

        // Name of the category
        public string Name { get; set; } = string.Empty;

        // Optional description of the category
        public string? Description { get; set; }

        // Restaurant Id tells which restaurant this category belongs to
        public Guid RestaurantId { get; set; }

        // Navigation property for related menu items inside this category
        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}