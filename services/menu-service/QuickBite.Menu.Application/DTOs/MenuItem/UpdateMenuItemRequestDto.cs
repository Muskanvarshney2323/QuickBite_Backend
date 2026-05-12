using System.ComponentModel.DataAnnotations;

namespace QuickBite.Menu.Application.DTOs.MenuItem
{
    // DTO used to update an existing menu item
    public class UpdateMenuItemRequestDto
    {
        // Updated item name
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Updated item description
        [MaxLength(500)]
        public string? Description { get; set; }

        // Updated item price
        [Range(0.01, 100000)]
        public decimal Price { get; set; }

        // Updated availability status
        public bool IsAvailable { get; set; }

        // Updated image URL
        public string? ImageUrl { get; set; }

        // Updated category id
        [Required]
        public Guid MenuCategoryId { get; set; }
    }
}