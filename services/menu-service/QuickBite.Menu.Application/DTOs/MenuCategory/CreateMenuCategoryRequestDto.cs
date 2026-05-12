using System.ComponentModel.DataAnnotations;

namespace QuickBite.Menu.Application.DTOs.MenuCategory
{
    // DTO used to create a new menu category
    public class CreateMenuCategoryRequestDto
    {
        // Category name
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Optional category description
        [MaxLength(300)]
        public string? Description { get; set; }

        // Restaurant id to which this category belongs
        [Required]
        public Guid RestaurantId { get; set; }
    }
}