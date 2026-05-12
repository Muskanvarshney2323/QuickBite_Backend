namespace QuickBite.Order.Application.DTOs;

/// <summary>
/// Request DTO used to assign a delivery agent to an order.
/// In production this would be invoked after Delivery-Service nominates an agent.
/// </summary>
public class AssignDeliveryAgentRequestDto
{
    public Guid DeliveryAgentId { get; set; }
}
