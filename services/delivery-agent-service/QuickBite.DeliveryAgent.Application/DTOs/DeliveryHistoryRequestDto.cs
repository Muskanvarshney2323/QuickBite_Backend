namespace QuickBite.DeliveryAgent.Application.DTOs
{
    public class DeliveryHistoryDto
    {
        public Guid OrderId { get; set; }
        public string? RestaurantName { get; set; }
        public string? CustomerName { get; set; }
        public string? DeliveryAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public DateTime DeliveredAt { get; set; }
    }
}