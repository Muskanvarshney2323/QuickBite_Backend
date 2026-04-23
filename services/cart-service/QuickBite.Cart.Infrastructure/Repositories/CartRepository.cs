using Microsoft.EntityFrameworkCore;
using QuickBite.Cart.Application.Interfaces;
using QuickBite.Cart.Domain.Entities;
using QuickBite.Cart.Infrastructure.Data;

namespace QuickBite.Cart.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of ICartRepository.
/// </summary>
public class CartRepository : ICartRepository
{
    private readonly CartDbContext _context;

    public CartRepository(CartDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Domain.Entities.Cart?> FindByCustomerIdAsync(Guid customerId)
    {
        return await _context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.CustomerId == customerId);
    }

    /// <inheritdoc />
    public async Task<Domain.Entities.Cart?> FindByCartIdAsync(Guid cartId)
    {
        return await _context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.Id == cartId);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByCustomerIdAsync(Guid customerId)
    {
        return await _context.Carts.AnyAsync(x => x.CustomerId == customerId);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Domain.Entities.Cart>> FindByRestaurantIdAsync(Guid restaurantId)
    {
        return await _context.Carts
            .Include(x => x.CartItems)
            .Where(x => x.RestaurantId == restaurantId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> DeleteByCustomerIdAsync(Guid customerId)
    {
        var cart = await _context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.CustomerId == customerId);

        if (cart is null) return false;

        // Cascade delete will take care of CartItems, but removing explicitly
        // keeps the intent obvious.
        _context.CartItems.RemoveRange(cart.CartItems);
        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Domain.Entities.Cart>> GetAllAsync()
    {
        return await _context.Carts
            .Include(x => x.CartItems)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddCartAsync(Domain.Entities.Cart cart)
    {
        await _context.Carts.AddAsync(cart);
    }

    /// <inheritdoc />
    public async Task AddCartItemAsync(CartItem cartItem)
    {
        await _context.CartItems.AddAsync(cartItem);
    }

    /// <inheritdoc />
    public async Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId)
    {
        return await _context.CartItems.FirstOrDefaultAsync(x => x.Id == cartItemId);
    }

    /// <inheritdoc />
    public void UpdateCart(Domain.Entities.Cart cart) => _context.Carts.Update(cart);

    /// <inheritdoc />
    public void UpdateCartItem(CartItem cartItem) => _context.CartItems.Update(cartItem);

    /// <inheritdoc />
    public void RemoveCartItem(CartItem cartItem) => _context.CartItems.Remove(cartItem);

    /// <inheritdoc />
    public void ClearItems(Domain.Entities.Cart cart) => _context.CartItems.RemoveRange(cart.CartItems);

    /// <inheritdoc />
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
