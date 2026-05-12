using QuickBite.DeliveryAgent.Domain.Enums;

namespace QuickBite.DeliveryAgent.Application.DTOs;

/// <summary>
/// Request DTO used when a new delivery agent registers on the platform.
/// The agent must already have a user account in Auth-Service (UserId).
/// </summary>
public class RegisterAgentRequestDto
{
    /// <summary>Linked Auth-Service user id.</summary>
    public Guid UserId { get; set; }

    /// <summary>Full name of the agent.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Contact phone number.</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>Vehicle type (bike / scooter / bicycle / car).</summary>
    public VehicleType VehicleType { get; set; } = VehicleType.BIKE;

    /// <summary>Vehicle registration number.</summary>
    public string VehicleNumber { get; set; } = string.Empty;
}
