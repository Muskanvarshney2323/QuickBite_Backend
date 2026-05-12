using QuickBite.DeliveryAgent.Application.DTOs;
using QuickBite.DeliveryAgent.Application.Interfaces;

// Alias so "DeliveryAgentEntity" always means the class,
// not the "QuickBite.DeliveryAgent" namespace.
using DeliveryAgentEntity = QuickBite.DeliveryAgent.Domain.Entities.DeliveryAgent;

namespace QuickBite.DeliveryAgent.Application.Services;

/// <summary>
/// Business logic for delivery agents.
/// Handles register, location update, availability, verify, assign, complete, and rating.
/// </summary>
public class DeliveryService : IDeliveryService
{
    private readonly IDeliveryRepository _repository;

    public DeliveryService(IDeliveryRepository repository)
    {
        _repository = repository;
    }

    // Register a new agent. New agents start as not verified and not available.
    public async Task<DeliveryAgentResponseDto> RegisterAgentAsync(RegisterAgentRequestDto request)
    {
        var agent = new DeliveryAgentEntity
        {
            UserId = request.UserId,
            FullName = request.FullName,
            Phone = request.Phone,
            VehicleType = request.VehicleType,
            VehicleNumber = request.VehicleNumber,
            IsAvailable = false,
            IsVerified = false,
            AvgRating = 0,
            TotalDeliveries = 0
        };

        await _repository.AddAgentAsync(agent);
        await _repository.SaveChangesAsync();

        return MapToResponse(agent);
    }

    // Get an agent by their id.
    public async Task<DeliveryAgentResponseDto?> GetAgentByIdAsync(Guid agentId)
    {
        var agent = await _repository.FindByAgentIdAsync(agentId);
        return agent is null ? null : MapToResponse(agent);
    }

    // Get an agent by their linked user id.
    public async Task<DeliveryAgentResponseDto?> GetAgentByUserIdAsync(Guid userId)
    {
        var agent = await _repository.FindByUserIdAsync(userId);
        return agent is null ? null : MapToResponse(agent);
    }

    // Return all agents that are available and verified.
    public async Task<IReadOnlyList<DeliveryAgentResponseDto>> GetNearbyAgentsAsync(NearbyAgentsRequestDto request)
    {
        var agents = await _repository.FindNearbyAgentsAsync();
        return agents.Select(MapToResponse).ToList();
    }

    // Save the agent's latest (lat, lng).
    public async Task<DeliveryAgentResponseDto?> UpdateLocationAsync(Guid agentId, UpdateLocationRequestDto request)
    {
        var agent = await _repository.FindByAgentIdAsync(agentId);
        if (agent is null) return null;

        agent.CurrentLatitude = request.Latitude;
        agent.CurrentLongitude = request.Longitude;

        _repository.UpdateAgent(agent);
        await _repository.SaveChangesAsync();

        return MapToResponse(agent);
    }

    // Turn the agent's availability on or off.
    public async Task<DeliveryAgentResponseDto?> SetAvailabilityAsync(Guid agentId, SetAvailabilityRequestDto request)
    {
        var agent = await _repository.FindByAgentIdAsync(agentId);
        if (agent is null) return null;

        agent.IsAvailable = request.IsAvailable;

        _repository.UpdateAgent(agent);
        await _repository.SaveChangesAsync();

        return MapToResponse(agent);
    }

    // Admin marks the agent as verified.
    public async Task<DeliveryAgentResponseDto?> VerifyAgentAsync(Guid agentId)
    {
        var agent = await _repository.FindByAgentIdAsync(agentId);
        if (agent is null) return null;

        agent.IsVerified = true;

        _repository.UpdateAgent(agent);
        await _repository.SaveChangesAsync();

        return MapToResponse(agent);
    }

    // Order-Service calls this when giving an order to an agent.
    // The agent is marked busy (not available).
    public async Task<DeliveryAgentResponseDto?> AssignOrderAsync(Guid agentId, AssignOrderRequestDto request)
    {
        var agent = await _repository.FindByAgentIdAsync(agentId);
        if (agent is null) return null;

        agent.IsAvailable = false;

        _repository.UpdateAgent(agent);
        await _repository.SaveChangesAsync();

        return MapToResponse(agent);
    }

    // Agent marks the delivery as done.
    // Increase delivery count and make the agent available again.
    public async Task<DeliveryAgentResponseDto?> CompleteDeliveryAsync(Guid agentId, CompleteDeliveryRequestDto request)
    {
        var agent = await _repository.FindByAgentIdAsync(agentId);
        if (agent is null) return null;

        agent.TotalDeliveries = agent.TotalDeliveries + 1;
        agent.IsAvailable = true;

        _repository.UpdateAgent(agent);
        await _repository.SaveChangesAsync();

        return MapToResponse(agent);
    }

    // Save a new rating from the customer (just overwrite with the latest value for simplicity).
    public async Task<DeliveryAgentResponseDto?> UpdateRatingAsync(Guid agentId, UpdateRatingRequestDto request)
    {
        var agent = await _repository.FindByAgentIdAsync(agentId);
        if (agent is null) return null;

        agent.AvgRating = request.NewRating;

        _repository.UpdateAgent(agent);
        await _repository.SaveChangesAsync();

        return MapToResponse(agent);
    }

    // Return all verified agents who are currently busy on a delivery.
    public async Task<IReadOnlyList<DeliveryAgentResponseDto>> GetActiveDeliveriesAsync()
    {
        var verifiedAgents = await _repository.FindByIsVerifiedAsync(true);

        // Keep only the ones who are currently busy.
        var busyAgents = verifiedAgents.Where(a => a.IsAvailable == false).ToList();

        return busyAgents.Select(MapToResponse).ToList();
    }

    // Convert an entity into the response DTO that we send back to the client.
    private static DeliveryAgentResponseDto MapToResponse(DeliveryAgentEntity agent)
    {
        return new DeliveryAgentResponseDto
        {
            AgentId = agent.Id,
            UserId = agent.UserId,
            FullName = agent.FullName,
            Phone = agent.Phone,
            VehicleType = agent.VehicleType,
            VehicleNumber = agent.VehicleNumber,
            CurrentLatitude = agent.CurrentLatitude,
            CurrentLongitude = agent.CurrentLongitude,
            IsAvailable = agent.IsAvailable,
            IsVerified = agent.IsVerified,
            AvgRating = agent.AvgRating,
            TotalDeliveries = agent.TotalDeliveries
        };
    }
}