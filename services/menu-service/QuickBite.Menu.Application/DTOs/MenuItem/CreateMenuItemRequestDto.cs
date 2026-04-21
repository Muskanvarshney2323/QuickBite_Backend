using System.ComponentModel.DataAnnotations;

namespace QuickBite.Menu.Application.DTOs.MenuItem
{
    // DTO used to create a new menu item
    public class CreateMenuItemRequestDto
    {
        // Name of the menu item
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Description of the menu item
        [MaxLength(500)]
        public string? Description { get; set; }

        // Price of the item
        [Range(0.01, 100000)]
        public decimal Price { get; set; }

        // Item availability status
        public bool IsAvailable { get; set; } = true;

        // Optional item image URL
        public string? ImageUrl { get; set; }

        // Category id to which the item belongs
        [Required]
        public Guid MenuCategoryId { get; set; }
    }
}