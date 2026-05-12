using System.ComponentModel.DataAnnotations;

namespace QuickBite.Restaurant.Application.DTOs
{
    // DTO used when creating a new restaurant
    public class CreateRestaurantRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Cuisine { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public Guid OwnerId { get; set; }

        // Minimum order value for this restaurant
        public decimal MinimumOrderAmount { get; set; }

        // Estimated delivery time in minutes
        public int EstimatedDeliveryTimeInMinutes { get; set; }
    }
}