using QuickBite.Cart.Application.DTOs;

namespace QuickBite.Cart.Application.Interfaces;

/// <summary>
/// Service interface for cart business logic.
/// </summary>
public interface ICartService
{
    Task<CartResponseDto> AddItemToCartAsync(AddCartItemRequestDto request);
    Task<CartResponseDto?> GetCartByUserIdAsync(Guid userId);
    Task<CartResponseDto?> UpdateCartItemAsync(Guid cartItemId, UpdateCartItemRequestDto request);
    Task<bool> RemoveCartItemAsync(Guid cartItemId);
    Task<bool> ClearCartAsync(Guid userId);
}