using QuickBite.DeliveryAgent.Application.DTOs;

namespace QuickBite.DeliveryAgent.Application.Interfaces;

/// <summary>
/// Service contract for delivery-agent business logic.
/// Mirrors the Delivery Agent-Service class-diagram methods:
/// RegisterAgent, GetAgentById, GetAgentByUserId, GetNearbyAgents,
/// UpdateLocation, SetAvailability, VerifyAgent, AssignOrder,
/// CompleteDelivery, UpdateRating, GetActiveDeliveries.
/// </summary>
public interface IDeliveryService
{
    /// <summary>Register a new delivery agent (creates a profile linked to a user id).</summary>
    Task<DeliveryAgentResponseDto> RegisterAgentAsync(RegisterAgentRequestDto request);

    /// <summary>Fetch a single agent by id.</summary>
    Task<DeliveryAgentResponseDto?> GetAgentByIdAsync(Guid agentId);

    /// <summary>Fetch an agent by its Auth-Service user id.</summary>
    Task<DeliveryAgentResponseDto?> GetAgentByUserIdAsync(Guid userId);

    /// <summary>Find agents near a (lat, lng) that are available + verified.</summary>
    Task<IReadOnlyList<DeliveryAgentResponseDto>> GetNearbyAgentsAsync(NearbyAgentsRequestDto request);

    /// <summary>Push a new (lat, lng) for an agent (called from the mobile app).</summary>
    Task<DeliveryAgentResponseDto?> UpdateLocationAsync(Guid agentId, UpdateLocationRequestDto request);

    /// <summary>Toggle whether the agent is available to pick up a new delivery.</summary>
    Task<DeliveryAgentResponseDto?> SetAvailabilityAsync(Guid agentId, SetAvailabilityRequestDto request);

    /// <summary>Mark an agent as verified (admin-only workflow).</summary>
    Task<DeliveryAgentResponseDto?> VerifyAgentAsync(Guid agentId);

    /// <summary>
    /// Assign an order to the agent. The agent becomes "not available" (busy)
    /// until they call CompleteDelivery.
    /// </summary>
    Task<DeliveryAgentResponseDto?> AssignOrderAsync(Guid agentId, AssignOrderRequestDto request);

    /// <summary>
    /// Mark an order delivered by this agent. Increments TotalDeliveries and
    /// flips IsAvailable back on.
    /// </summary>
    Task<DeliveryAgentResponseDto?> CompleteDeliveryAsync(Guid agentId, CompleteDeliveryRequestDto request);

    /// <summary>Record a new customer rating and update the running average.</summary>
    Task<DeliveryAgentResponseDto?> UpdateRatingAsync(Guid agentId, UpdateRatingRequestDto request);

    /// <summary>All agents currently on an active delivery (not available).</summary>
    Task<IReadOnlyList<DeliveryAgentResponseDto>> GetActiveDeliveriesAsync();

    /// <summary>Fetch the delivery history of an agent.</summary>
    Task<IEnumerable<object>> GetDeliveryHistoryAsync(Guid agentId);
}
