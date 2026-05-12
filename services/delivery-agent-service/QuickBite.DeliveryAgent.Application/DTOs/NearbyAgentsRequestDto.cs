namespace QuickBite.DeliveryAgent.Application.DTOs;

/// <summary>
/// Query DTO for the "find nearby agents" endpoint.
/// Given a restaurant's (lat, lng) and a radius in km, we return agents
/// that are available, verified and within the radius.
/// </summary>
public class NearbyAgentsRequestDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    /// <summary>Search radius in kilometres (default 5km).</summary>
    public double RadiusKm { get; set; } = 5.0;
}
