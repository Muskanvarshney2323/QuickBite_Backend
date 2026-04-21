namespace QuickBite.Menu.Application.DTOs.MenuCategory
{
    // DTO returned when category data is sent to the client
    public class MenuCategoryResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid RestaurantId { get; set; }
    }
}