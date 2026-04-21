using QuickBite.Cart.Domain.Entities;

namespace QuickBite.Cart.Application.Interfaces;

/// <summary>
/// Repository interface used for cart database operations.
/// </summary>
public interface ICartRepository
{
    Task<QuickBite.Cart.Domain.Entities.Cart?> GetCartByUserIdAsync(Guid userId);
    Task<QuickBite.Cart.Domain.Entities.Cart?> GetCartByIdAsync(Guid cartId);
    Task AddCartAsync(QuickBite.Cart.Domain.Entities.Cart cart);
    Task AddCartItemAsync(CartItem cartItem);
    Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId);
    Task UpdateCartAsync(QuickBite.Cart.Domain.Entities.Cart cart);
    Task UpdateCartItemAsync(CartItem cartItem);
    Task RemoveCartItemAsync(CartItem cartItem);
    Task ClearCartAsync(QuickBite.Cart.Domain.Entities.Cart cart);
    Task SaveChangesAsync();
}