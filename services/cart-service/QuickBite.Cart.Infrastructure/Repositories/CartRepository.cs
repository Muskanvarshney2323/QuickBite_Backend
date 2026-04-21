using Microsoft.EntityFrameworkCore;
using QuickBite.Cart.Application.Interfaces;
using QuickBite.Cart.Domain.Entities;
using QuickBite.Cart.Infrastructure.Data;

namespace QuickBite.Cart.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for cart-related database operations.
/// </summary>
public class CartRepository : ICartRepository
{
    private readonly CartDbContext _context;

    public CartRepository(CartDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Returns cart details by user id including cart items.
    /// </summary>
    public async Task<QuickBite.Cart.Domain.Entities.Cart?> GetCartByUserIdAsync(Guid userId)
    {
        return await _context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    /// <summary>
    /// Returns cart by cart id including items.
    /// </summary>
    public async Task<QuickBite.Cart.Domain.Entities.Cart?> GetCartByIdAsync(Guid cartId)
    {
        return await _context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.Id == cartId);
    }

    /// <summary>
    /// Adds a new cart into database.
    /// </summary>
    public async Task AddCartAsync(QuickBite.Cart.Domain.Entities.Cart cart)
    {
        await _context.Carts.AddAsync(cart);
    }

    /// <summary>
    /// Adds a new cart item into database.
    /// </summary>
    public async Task AddCartItemAsync(CartItem cartItem)
    {
        await _context.CartItems.AddAsync(cartItem);
    }

    /// <summary>
    /// Returns a cart item by its id.
    /// </summary>
    public async Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId)
    {
        return await _context.CartItems.FirstOrDefaultAsync(x => x.Id == cartItemId);
    }

    /// <summary>
    /// Marks cart entity as updated.
    /// </summary>
    public Task UpdateCartAsync(QuickBite.Cart.Domain.Entities.Cart cart)
    {
        _context.Carts.Update(cart);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Marks cart item entity as updated.
    /// </summary>
    public Task UpdateCartItemAsync(CartItem cartItem)
    {
        _context.CartItems.Update(cartItem);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes a single cart item.
    /// </summary>
    public Task RemoveCartItemAsync(CartItem cartItem)
    {
        _context.CartItems.Remove(cartItem);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes all items from a cart.
    /// </summary>
    public Task ClearCartAsync(QuickBite.Cart.Domain.Entities.Cart cart)
    {
        _context.CartItems.RemoveRange(cart.CartItems);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Saves all pending changes to database.
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}