using QuickBite.Cart.Application.DTOs;
using QuickBite.Cart.Application.Interfaces;
using QuickBite.Cart.Domain.Entities;

namespace QuickBite.Cart.Application.Services;

/// <summary>
/// Handles business logic for cart operations.
/// </summary>
public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    /// <summary>
    /// Adds a new item to the user's cart.
    /// If cart does not exist, it creates a new cart first.
    /// If the same menu item already exists, it increases the quantity.
    /// </summary>
    public async Task<CartResponseDto> AddItemToCartAsync(AddCartItemRequestDto request)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(request.UserId);

        if (cart == null)
        {
            cart = new Domain.Entities.Cart
            {
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cartRepository.AddCartAsync(cart);
            await _cartRepository.SaveChangesAsync();
        }

        var existingItem = cart.CartItems.FirstOrDefault(x => x.MenuItemId == request.MenuItemId);

        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
            await _cartRepository.UpdateCartItemAsync(existingItem);
        }
        else
        {
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                MenuItemId = request.MenuItemId,
                MenuItemName = request.MenuItemName,
                UnitPrice = request.UnitPrice,
                Quantity = request.Quantity
            };

            await _cartRepository.AddCartItemAsync(cartItem);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _cartRepository.UpdateCartAsync(cart);
        await _cartRepository.SaveChangesAsync();

        var updatedCart = await _cartRepository.GetCartByUserIdAsync(request.UserId);
        return MapCartToResponse(updatedCart!);
    }

    /// <summary>
    /// Returns full cart details for a specific user.
    /// </summary>
    public async Task<CartResponseDto?> GetCartByUserIdAsync(Guid userId)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);

        if (cart == null)
            return null;

        return MapCartToResponse(cart);
    }

    /// <summary>
    /// Updates quantity of a cart item.
    /// </summary>
    public async Task<CartResponseDto?> UpdateCartItemAsync(Guid cartItemId, UpdateCartItemRequestDto request)
    {
        var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);

        if (cartItem == null)
            return null;

        cartItem.Quantity = request.Quantity;
        await _cartRepository.UpdateCartItemAsync(cartItem);

        var cart = await _cartRepository.GetCartByIdAsync(cartItem.CartId);
        if (cart != null)
        {
            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateCartAsync(cart);
        }

        await _cartRepository.SaveChangesAsync();

        var updatedCart = await _cartRepository.GetCartByIdAsync(cartItem.CartId);
        return updatedCart == null ? null : MapCartToResponse(updatedCart);
    }

    /// <summary>
    /// Removes one item from the cart.
    /// </summary>
    public async Task<bool> RemoveCartItemAsync(Guid cartItemId)
    {
        var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);

        if (cartItem == null)
            return false;

        var cart = await _cartRepository.GetCartByIdAsync(cartItem.CartId);

        await _cartRepository.RemoveCartItemAsync(cartItem);

        if (cart != null)
        {
            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateCartAsync(cart);
        }

        await _cartRepository.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Removes all items from the user's cart.
    /// </summary>
    public async Task<bool> ClearCartAsync(Guid userId)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);

        if (cart == null)
            return false;

        await _cartRepository.ClearCartAsync(cart);
        cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.UpdateCartAsync(cart);
        await _cartRepository.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Converts Cart entity into response DTO.
    /// </summary>
    private static CartResponseDto MapCartToResponse(Domain.Entities.Cart cart)
    {
        var items = cart.CartItems.Select(item => new CartItemResponseDto
        {
            CartItemId = item.Id,
            MenuItemId = item.MenuItemId,
            MenuItemName = item.MenuItemName,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity,
            TotalPrice = item.UnitPrice * item.Quantity
        }).ToList();

        return new CartResponseDto
        {
            CartId = cart.Id,
            UserId = cart.UserId,
            Items = items,
            GrandTotal = items.Sum(x => x.TotalPrice)
        };
    }
}