// Import for VehicleType enum
using QuickBite.DeliveryAgent.Domain.Enums;

// Namespace for Data Transfer Objects (Request/Response)
namespace QuickBite.DeliveryAgent.Application.DTOs;

// ========================= DELIVERY AGENT RESPONSE DTO =========================
/// <summary>
/// DeliveryAgentResponseDto: Response DTO for delivery agent details
/// Used in GET /api/v1/agents/{id} and list endpoints
/// Projects DeliveryAgent entity into safe shape for client delivery
/// </summary>
public class DeliveryAgentResponseDto
{
    // ========================= AGENT ID =========================
    // AgentId: Unique identifier for this delivery agent
    // Type: GUID
    // Example: "550e8400-e29b-41d4-a716-446655440000"
    // Database primary key
    public Guid AgentId { get; set; }

    // ========================= USER ID =========================
    // UserId: Linked user account in Auth Service
    // Type: GUID
    // Example: "660e8400-e29b-41d4-a716-446655440000"
    // Used for authentication and authorization
    public Guid UserId { get; set; }

    // ========================= FULL NAME =========================
    // FullName: Delivery agent's complete name
    // Type: String
    // Example: "Raj Kumar"
    // Shown to customers as driver identity
    public string FullName { get; set; } = string.Empty;

    // ========================= PHONE =========================
    // Phone: Contact number for delivery agent
    // Type: String
    // Example: "+91-98765-43210"
    // Used for communication during delivery
    public string Phone { get; set; } = string.Empty;

    // ========================= VEHICLE TYPE =========================
    // VehicleType: Type of vehicle used for delivery
    // Type: VehicleType enum
    // Example values:
    //   BIKE = Two-wheeler motorcycle
    //   SCOOTER = Two-wheeler scooter
    //   BICYCLE = Human-powered bicycle
    //   CAR = Four-wheeler vehicle
    // Used for: Showing delivery method to customer
    public VehicleType VehicleType { get; set; }

    // ========================= VEHICLE NUMBER =========================
    // VehicleNumber: Registration plate number of vehicle
    // Type: String
    // Example: "DL01AB1234"
    // Used for tracking and verification
    public string VehicleNumber { get; set; } = string.Empty;

    // ========================= CURRENT LATITUDE =========================
    // CurrentLatitude: GPS latitude coordinate of agent's current location
    // Type: Double
    // Example: 28.6139 (for Delhi)
    // Updated frequently for real-time tracking
    public double CurrentLatitude { get; set; }

    // ========================= CURRENT LONGITUDE =========================
    // CurrentLongitude: GPS longitude coordinate of agent's current location
    // Type: Double
    // Example: 77.2090 (for Delhi)
    // Updated frequently for real-time tracking
    public double CurrentLongitude { get; set; }

    // ========================= IS AVAILABLE =========================
    // IsAvailable: Whether agent is currently available for deliveries
    // Type: Boolean
    // Example: true (online, accepting orders), false (offline/on break)
    // Business Logic: Only available agents shown to assignment system
    public bool IsAvailable { get; set; }

    // ========================= IS VERIFIED =========================
    // IsVerified: Whether agent's documents are verified
    // Type: Boolean
    // Example: true (identity and vehicle verified), false (pending verification)
    // Business Logic: Verified agents shown to customers
    public bool IsVerified { get; set; }

    // ========================= AVG RATING =========================
    // AvgRating: Average delivery service rating from customer reviews
    // Type: Double (0.0 to 5.0)
    // Example: 4.7 (excellent delivery service)
    // Calculated from: All customer reviews' DeliveryRating average
    // Used for: Agent performance tracking
    public double AvgRating { get; set; }

    // ========================= TOTAL DELIVERIES =========================
    // TotalDeliveries: Number of successful deliveries completed
    // Type: Integer
    // Example: 1250 (agent completed 1250 deliveries)
    // Used for: Experience indicator to customers
    public int TotalDeliveries { get; set; }
}
