namespace QuickBite.Restaurant.Domain.Entities
{
    // Main Restaurant entity stored in the database
    public class Restaurant
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Cuisine { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public Guid OwnerId { get; set; }

        // Shows whether admin approved the restaurant or not
        public bool IsApproved { get; set; } = false;

        // Shows whether restaurant is currently open or closed
        public bool IsOpen { get; set; } = true;

        // Minimum order amount required to place an order
        public decimal MinimumOrderAmount { get; set; }

        // Estimated delivery time in minutes
        public int EstimatedDeliveryTimeInMinutes { get; set; }

        // Average rating of restaurant
        public double AverageRating { get; set; }
    }
}