using DeliveryAgentEntity = QuickBite.DeliveryAgent.Domain.Entities.DeliveryAgent;

namespace QuickBite.DeliveryAgent.Application.Interfaces;
/// <summary>
/// Repository contract for saving and loading delivery agents from the database.
/// </summary>
public interface IDeliveryRepository
{
    // Find an agent by the linked user id.
    Task<DeliveryAgentEntity?> FindByUserIdAsync(Guid userId);
    Task<DeliveryAgentEntity?> FindByAgentIdAsync(Guid agentId);
    Task<IReadOnlyList<DeliveryAgentEntity>> FindByIsAvailableAsync(bool isAvailable);
    Task<IReadOnlyList<DeliveryAgentEntity>> FindByIsVerifiedAsync(bool isVerified);
    Task<IReadOnlyList<DeliveryAgentEntity>> FindNearbyAgentsAsync();
    Task AddAgentAsync(DeliveryAgentEntity agent);
    void UpdateAgent(DeliveryAgentEntity agent);

    // Save all pending changes.
    Task SaveChangesAsync();
}
