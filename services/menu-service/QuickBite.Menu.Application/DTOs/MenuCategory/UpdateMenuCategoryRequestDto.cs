using System.ComponentModel.DataAnnotations;

namespace QuickBite.Menu.Application.DTOs.MenuCategory
{
    // DTO used to update an existing menu category
    public class UpdateMenuCategoryRequestDto
    {
        // Updated category name
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Updated category description
        [MaxLength(300)]
        public string? Description { get; set; }
    }
}