using Microsoft.EntityFrameworkCore;
using QuickBite.Order.Application.Interfaces;
using QuickBite.Order.Domain.Enums;
using QuickBite.Order.Infrastructure.Data;

namespace QuickBite.Order.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IOrderRepository.
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Domain.Entities.Order?> FindByOrderIdAsync(Guid orderId)
    {
        return await _context.Orders
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == orderId);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Domain.Entities.Order>> FindByCustomerIdAsync(Guid customerId)
    {
        return await _context.Orders
            .Include(x => x.OrderItems)
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.OrderDate)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Domain.Entities.Order>> FindByRestaurantIdAsync(Guid restaurantId)
    {
        return await _context.Orders
            .Include(x => x.OrderItems)
            .Where(x => x.RestaurantId == restaurantId)
            .OrderByDescending(x => x.OrderDate)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Domain.Entities.Order>> FindByOrderStatusAsync(OrderStatus status)
    {
        return await _context.Orders
            .Include(x => x.OrderItems)
            .Where(x => x.OrderStatus == status)
            .OrderByDescending(x => x.OrderDate)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Domain.Entities.Order>> FindByDeliveryAgentIdAsync(Guid deliveryAgentId)
    {
        return await _context.Orders
            .Include(x => x.OrderItems)
            .Where(x => x.DeliveryAgentId == deliveryAgentId)
            .OrderByDescending(x => x.OrderDate)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Domain.Entities.Order>> FindByOrderDateBetweenAsync(DateTime fromUtc, DateTime toUtc)
    {
        return await _context.Orders
            .Include(x => x.OrderItems)
            .Where(x => x.OrderDate >= fromUtc && x.OrderDate <= toUtc)
            .OrderByDescending(x => x.OrderDate)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<int> CountByRestaurantIdAsync(Guid restaurantId)
    {
        return await _context.Orders.CountAsync(x => x.RestaurantId == restaurantId);
    }

    /// <inheritdoc />
    public async Task AddOrderAsync(Domain.Entities.Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    /// <inheritdoc />
    public void UpdateOrder(Domain.Entities.Order order) => _context.Orders.Update(order);

    /// <inheritdoc />
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
