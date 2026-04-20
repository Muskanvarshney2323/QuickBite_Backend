namespace QuickBite.Restaurant.Domain.Entities
{
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
        public bool IsApproved { get; set; } = false;
        public bool IsOpen { get; set; } = true;
    }
}