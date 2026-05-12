using QuickBite.Order.Domain.Enums;

namespace QuickBite.Order.Application.DTOs;

/// <summary>
/// Request DTO used to update an order's lifecycle status.
/// </summary>
public class UpdateStatusRequestDto
{
    public OrderStatus NewStatus { get; set; }
}
