using QuickBite.DeliveryAgent.Domain.Enums;

namespace QuickBite.DeliveryAgent.Application.DTOs;

/// <summary>
/// Response DTO returned by all agent-related endpoints.
/// Projects the DeliveryAgent entity into a shape that is safe to send to clients.
/// </summary>
public class DeliveryAgentResponseDto
{
    public Guid AgentId { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    public string VehicleNumber { get; set; } = string.Empty;
    public double CurrentLatitude { get; set; }
    public double CurrentLongitude { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsVerified { get; set; }
    public double AvgRating { get; set; }
    public int TotalDeliveries { get; set; }
}
