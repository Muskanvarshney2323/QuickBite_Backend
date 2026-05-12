using QuickBite.Cart.Application.DTOs;

namespace QuickBite.Cart.Application.Interfaces;

/// <summary>
/// Service contract for cart business logic.
/// Mirrors the Cart-Service class-diagram methods:
/// GetCartByCustomer, AddItem, RemoveItem, UpdateQuantity,
/// ClearCart, CartTotal, ChangeRestaurant, ApplyPromoCode, GetAllCarts.
/// </summary>
public interface ICartService
{
    /// <summary>Fetch the active cart for a customer (or null if none).</summary>
    Task<CartResponseDto?> GetCartByCustomerAsync(Guid customerId);

    /// <summary>
    /// Add an item to the customer's cart.
    /// Creates the cart if it does not yet exist.
    /// Rejects the add if the item's restaurant differs from the cart's restaurant
    /// (single-restaurant ordering rule).
    /// </summary>
    Task<CartResponseDto> AddItemAsync(AddCartItemRequestDto request);

    /// <summary>Remove a single item line from a cart. Returns true if something was removed.</summary>
    Task<bool> RemoveItemAsync(Guid cartItemId);

    /// <summary>Update the quantity of an existing cart item.</summary>
    Task<CartResponseDto?> UpdateQuantityAsync(Guid cartItemId, UpdateCartItemRequestDto request);

    /// <summary>Remove all items from a customer's cart.</summary>
    Task<bool> ClearCartAsync(Guid customerId);

    /// <summary>Compute and return the current total price of a customer's cart.</summary>
    Task<decimal> CartTotalAsync(Guid customerId);

    /// <summary>
    /// Switch the customer's cart to a different restaurant.
    /// All existing items are cleared because only one restaurant is allowed per cart.
    /// </summary>
    Task<CartResponseDto> ChangeRestaurantAsync(ChangeRestaurantRequestDto request);

    /// <summary>Apply a promo code to the customer's cart. Returns the updated cart.</summary>
    Task<CartResponseDto?> ApplyPromoCodeAsync(ApplyPromoCodeRequestDto request);

    /// <summary>Return all carts (admin / diagnostics).</summary>
    Task<IReadOnlyList<CartResponseDto>> GetAllCartsAsync();
}
