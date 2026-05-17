// Import for VehicleType enum
using QuickBite.DeliveryAgent.Domain.Enums;

// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.DeliveryAgent.Application.DTOs;

// ========================= REGISTER AGENT REQUEST DTO =========================
/// <summary>
/// RegisterAgentRequestDto: Request DTO when delivery partner registers on platform
/// Used in POST /api/v1/agents/register endpoint
/// Agent must already have user account in Auth-Service before registration
/// </summary>
public class RegisterAgentRequestDto
{
    // ========================= USER ID =========================
    /// <summary>Linked Auth-Service user id.</summary>
    // Type: GUID
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // Requirement: User must already exist in Auth Service
    // Business Logic: Links delivery agent to existing user account
    // Used for: Authentication and authorization
    // Required: Yes (cannot be empty)
    public Guid UserId { get; set; }

    // ========================= FULL NAME =========================
    /// <summary>Full name of the agent.</summary>
    // Type: String
    // Example: "Raj Kumar Sharma"
    // Shown to customers as delivery partner identity
    // Validation: Non-empty string
    // Required: Yes (should not be empty)
    public string FullName { get; set; } = string.Empty;

    // ========================= PHONE =========================
    /// <summary>Contact phone number.</summary>
    // Type: String
    // Example: "+91-98765-43210"
    // Used for customer communication during delivery
    // Validation: Valid phone format required
    // Required: Yes (should not be empty)
    public string Phone { get; set; } = string.Empty;

    // ========================= VEHICLE TYPE =========================
    /// <summary>Vehicle type (bike / scooter / bicycle / car).</summary>
    // Type: VehicleType enum
    // Example values:
    //   BIKE = Two-wheeler motorcycle (fast, for short/medium distances)
    //   SCOOTER = Two-wheeler scooter (economical)
    //   BICYCLE = Human-powered bicycle (eco-friendly, limited distance)
    //   CAR = Four-wheeler vehicle (for bulk orders)
    // Default: BIKE (most common)
    // Business Logic: Affects delivery speed calculation
    // Required: No (optional, defaults to BIKE)
    public VehicleType VehicleType { get; set; } = VehicleType.BIKE;

    // ========================= VEHICLE NUMBER =========================
    /// <summary>Vehicle registration number.</summary>
    // Type: String
    // Example: "DL01AB1234"
    // Format: Region code + 2 letters + 4 numbers (India standard)
    // Used for: Identification and compliance tracking
    // Validation: Non-empty string
    // Required: Yes (should not be empty)
    public string VehicleNumber { get; set; } = string.Empty;
}
