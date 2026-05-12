using QuickBite.DeliveryAgent.Domain.Common;
using QuickBite.DeliveryAgent.Domain.Enums;

namespace QuickBite.DeliveryAgent.Domain.Entities;

/// <summary>
/// A delivery agent who picks up food from restaurants and delivers it to customers.
/// Stores the agent's personal info, vehicle, current location, status, and performance.
/// </summary>
public class DeliveryAgent : BaseEntity
{
    // Id (from BaseEntity) is the agent id.

    /// <summary>The login account in Auth-Service that owns this agent profile.</summary>
    public Guid UserId { get; set; }

    /// <summary>Agent's full name.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Phone number for contact.</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>Type of vehicle (bike, scooter, bicycle, or car).</summary>
    public VehicleType VehicleType { get; set; } = VehicleType.BIKE;

    /// <summary>Vehicle registration number.</summary>
    public string VehicleNumber { get; set; } = string.Empty;

    /// <summary>Agent's current latitude (updated live from the mobile app).</summary>
    public double CurrentLatitude { get; set; }

    /// <summary>Agent's current longitude (updated live from the mobile app).</summary>
    public double CurrentLongitude { get; set; }

    /// <summary>True if the agent is free and can accept a new delivery.</summary>
    public bool IsAvailable { get; set; }

    /// <summary>True once the admin has verified the agent's documents.</summary>
    public bool IsVerified { get; set; }

    /// <summary>Average customer rating (0 to 5).</summary>
    public double AvgRating { get; set; }

    /// <summary>How many deliveries the agent has completed.</summary>
    public int TotalDeliveries { get; set; }
}
