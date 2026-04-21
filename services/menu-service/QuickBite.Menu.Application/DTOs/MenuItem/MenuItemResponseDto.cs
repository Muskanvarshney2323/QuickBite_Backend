namespace QuickBite.Menu.Application.DTOs.MenuItem
{
    // DTO returned when menu item data is sent to the client
    public class MenuItemResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public Guid MenuCategoryId { get; set; }
    }
}