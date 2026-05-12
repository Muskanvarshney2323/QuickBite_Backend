using System.ComponentModel.DataAnnotations;

namespace QuickBite.Restaurant.Application.DTOs
{
    // DTO used when updating restaurant details
    public class UpdateRestaurantRequestDto
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

        // Updated minimum order amount
        public decimal MinimumOrderAmount { get; set; }

        // Updated estimated delivery time in minutes
        public int EstimatedDeliveryTimeInMinutes { get; set; }
    }
}