namespace QuickBite.DeliveryAgent.Application.DTOs;

/// <summary>
/// Request DTO for pushing a new (latitude, longitude) from the agent's
/// mobile device. Called continuously while the agent is on a delivery.
/// </summary>
public class UpdateLocationRequestDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
