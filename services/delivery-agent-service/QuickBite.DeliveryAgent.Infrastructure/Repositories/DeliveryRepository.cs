using Microsoft.EntityFrameworkCore;
using QuickBite.DeliveryAgent.Application.Interfaces;
using QuickBite.DeliveryAgent.Infrastructure.Data;

// Alias the entity so "DeliveryAgentEntity" always means the class,
// not the "QuickBite.DeliveryAgent" namespace.
using DeliveryAgentEntity = QuickBite.DeliveryAgent.Domain.Entities.DeliveryAgent;

namespace QuickBite.DeliveryAgent.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the agent repository.
/// All database queries live here.
/// </summary>
public class DeliveryRepository : IDeliveryRepository
{
    private readonly DeliveryAgentDbContext _context;

    public DeliveryRepository(DeliveryAgentDbContext context)
    {
        _context = context;
    }

    // Find an agent by their linked user id.
    public async Task<DeliveryAgentEntity?> FindByUserIdAsync(Guid userId)
    {
        return await _context.DeliveryAgents
            .FirstOrDefaultAsync(a => a.UserId == userId);
    }

    // Find an agent by their agent id.
    public async Task<DeliveryAgentEntity?> FindByAgentIdAsync(Guid agentId)
    {
        return await _context.DeliveryAgents
            .FirstOrDefaultAsync(a => a.Id == agentId);
    }

    // Return all agents matching the availability flag.
    public async Task<IReadOnlyList<DeliveryAgentEntity>> FindByIsAvailableAsync(bool isAvailable)
    {
        return await _context.DeliveryAgents
            .Where(a => a.IsAvailable == isAvailable)
            .ToListAsync();
    }

    // Return all agents matching the verified flag.
    public async Task<IReadOnlyList<DeliveryAgentEntity>> FindByIsVerifiedAsync(bool isVerified)
    {
        return await _context.DeliveryAgents
            .Where(a => a.IsVerified == isVerified)
            .ToListAsync();
    }

    // Return all agents who are available and verified.
    public async Task<IReadOnlyList<DeliveryAgentEntity>> FindNearbyAgentsAsync()
    {
        return await _context.DeliveryAgents
            .Where(a => a.IsAvailable && a.IsVerified)
            .ToListAsync();
    }

    // Add a new agent to the DbSet (not saved until SaveChanges).
    public async Task AddAgentAsync(DeliveryAgentEntity agent)
    {
        await _context.DeliveryAgents.AddAsync(agent);
    }

    // Mark the agent as updated so EF tracks the changes.
    public void UpdateAgent(DeliveryAgentEntity agent)
    {
        _context.DeliveryAgents.Update(agent);
    }

    // Commit all pending changes to the database.
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}