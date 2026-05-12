using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickBite.DeliveryAgent.Application.DTOs;
using QuickBite.DeliveryAgent.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace QuickBite.DeliveryAgent.API.Controllers;

/// <summary>
/// API endpoints for delivery agents.
/// Route: /api/v1/agents
/// </summary>
[ApiController]
[Route("api/v1/agents")]
[Authorize]
[SwaggerTag("Delivery-agent operations: register, get, update location, availability, verify, assign, complete, rating")]
public class DeliveryController : ControllerBase
{
    private readonly IDeliveryService _deliveryService;

    public DeliveryController(IDeliveryService deliveryService)
    {
        _deliveryService = deliveryService;
    }

    // Register a new agent.
    [HttpPost]
    [SwaggerOperation(Summary = "Register agent")]
    public async Task<IActionResult> RegisterAgent([FromBody] RegisterAgentRequestDto request)
    {
        var result = await _deliveryService.RegisterAgentAsync(request);
        return CreatedAtAction(nameof(GetAgentById), new { agentId = result.AgentId }, result);
    }

    // Get an agent by id.
    [HttpGet("{agentId:guid}")]
    [SwaggerOperation(Summary = "Get agent by id")]
    public async Task<IActionResult> GetAgentById(Guid agentId)
    {
        var result = await _deliveryService.GetAgentByIdAsync(agentId);
        if (result is null) return NotFound("Agent not found.");
        return Ok(result);
    }

    // Get an agent by user id.
    [HttpGet("user/{userId:guid}")]
    [SwaggerOperation(Summary = "Get agent by user id")]
    public async Task<IActionResult> GetAgentByUserId(Guid userId)
    {
        var result = await _deliveryService.GetAgentByUserIdAsync(userId);
        if (result is null) return NotFound("Agent not found.");
        return Ok(result);
    }

    // Get all available and verified agents.
    [HttpGet("nearby")]
    [SwaggerOperation(Summary = "Get nearby agents")]
    public async Task<IActionResult> GetNearbyAgents()
    {
        var request = new NearbyAgentsRequestDto();
        var result = await _deliveryService.GetNearbyAgentsAsync(request);
        return Ok(result);
    }

    // Update the agent's latest location.
    [HttpPut("{agentId:guid}/location")]
    [SwaggerOperation(Summary = "Update location")]
    public async Task<IActionResult> UpdateLocation(Guid agentId, [FromBody] UpdateLocationRequestDto request)
    {
        var result = await _deliveryService.UpdateLocationAsync(agentId, request);
        if (result is null) return NotFound("Agent not found.");
        return Ok(result);
    }

    // Turn availability on or off.
    [HttpPut("{agentId:guid}/availability")]
    [SwaggerOperation(Summary = "Set availability")]
    public async Task<IActionResult> SetAvailability(Guid agentId, [FromBody] SetAvailabilityRequestDto request)
    {
        var result = await _deliveryService.SetAvailabilityAsync(agentId, request);
        if (result is null) return NotFound("Agent not found.");
        return Ok(result);
    }

    // Admin verifies the agent.
    [HttpPut("{agentId:guid}/verify")]
    [SwaggerOperation(Summary = "Verify agent")]
    public async Task<IActionResult> VerifyAgent(Guid agentId)
    {
        var result = await _deliveryService.VerifyAgentAsync(agentId);
        if (result is null) return NotFound("Agent not found.");
        return Ok(result);
    }

    // Assign an order to the agent (agent becomes busy).
    [HttpPost("{agentId:guid}/assignOrder")]
    [SwaggerOperation(Summary = "Assign order")]
    public async Task<IActionResult> AssignOrder(Guid agentId, [FromBody] AssignOrderRequestDto request)
    {
        var result = await _deliveryService.AssignOrderAsync(agentId, request);
        if (result is null) return NotFound("Agent not found.");
        return Ok(result);
    }

    // Mark the delivery as complete (agent becomes available again).
    [HttpPost("{agentId:guid}/completeDelivery")]
    [SwaggerOperation(Summary = "Complete delivery")]
    public async Task<IActionResult> CompleteDelivery(Guid agentId, [FromBody] CompleteDeliveryRequestDto request)
    {
        var result = await _deliveryService.CompleteDeliveryAsync(agentId, request);
        if (result is null) return NotFound("Agent not found.");
        return Ok(result);
    }

    // Save a new customer rating.
    [HttpPut("{agentId:guid}/rating")]
    [SwaggerOperation(Summary = "Update rating")]
    public async Task<IActionResult> UpdateRating(Guid agentId, [FromBody] UpdateRatingRequestDto request)
    {
        var result = await _deliveryService.UpdateRatingAsync(agentId, request);
        if (result is null) return NotFound("Agent not found.");
        return Ok(result);
    }

    // Get all agents currently on a delivery.
    [HttpGet("activeDeliveries")]
    [SwaggerOperation(Summary = "Get active deliveries")]
    public async Task<IActionResult> GetActiveDeliveries()
    {
        var result = await _deliveryService.GetActiveDeliveriesAsync();
        return Ok(result);
    }
}
