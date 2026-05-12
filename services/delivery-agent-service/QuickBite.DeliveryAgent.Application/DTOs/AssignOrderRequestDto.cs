namespace QuickBite.DeliveryAgent.Application.DTOs;

/// <summary>
/// Request DTO used by the Order-Service to reserve a specific agent for a given order.
/// The agent transitions to "on delivery" (not available) once the assignment succeeds.
/// </summary>
public class AssignOrderRequestDto
{
    /// <summary>Order being assigned to the agent.</summary>
    public Guid OrderId { get; set; }
}
