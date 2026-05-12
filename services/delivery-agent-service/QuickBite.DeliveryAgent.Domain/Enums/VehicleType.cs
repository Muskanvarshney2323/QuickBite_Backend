namespace QuickBite.DeliveryAgent.Domain.Enums;

/// <summary>
/// The type of vehicle a delivery agent uses for deliveries.
/// Stored as an int in the database so it is readable across providers.
/// </summary>
public enum VehicleType
{
    BIKE = 0,      // two-wheeler motorbike (most common)
    SCOOTER = 1,   // scooter / moped
    BICYCLE = 2,   // pedal bicycle, used for short-range deliveries
    CAR = 3        // car, used in some premium zones
}
