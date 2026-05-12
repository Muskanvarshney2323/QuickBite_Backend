namespace QuickBite.DeliveryAgent.Application.DTOs;

/// <summary>
/// Request DTO for toggling whether the agent is available to take a new delivery.
/// </summary>
public class SetAvailabilityRequestDto
{
    public bool IsAvailable { get; set; }
}
