namespace QuickBite.Restaurant.Application.DTOs
{
    // DTO used for sending restaurant data in API responses
    public class RestaurantResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Cuisine { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public Guid OwnerId { get; set; }

        public bool IsApproved { get; set; }

        public bool IsOpen { get; set; }

        public decimal MinimumOrderAmount { get; set; }

        public int EstimatedDeliveryTimeInMinutes { get; set; }

        public double AverageRating { get; set; }
    }
}