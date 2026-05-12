using QuickBite.Cart.Application.DTOs;
using QuickBite.Cart.Application.Interfaces;
using QuickBite.Cart.Domain.Entities;

namespace QuickBite.Cart.Application.Services;

/// <summary>
/// Business logic for cart operations.
/// Enforces: one active cart per customer, all items bound to the same restaurant,
/// server-computed TotalPrice, and price snapshots captured at the time of add.
/// </summary>
public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    /// <inheritdoc />
    public async Task<CartResponseDto?> GetCartByCustomerAsync(Guid customerId)
    {
        var cart = await _cartRepository.FindByCustomerIdAsync(customerId);
        return cart is null ? null : MapCartToResponse(cart);
    }

    /// <inheritdoc />
    public async Task<CartResponseDto> AddItemAsync(AddCartItemRequestDto request)
    {
        var cart = await _cartRepository.FindByCustomerIdAsync(request.CustomerId);

        if (cart is null)
        {
            // No cart yet - create one bound to this restaurant.
            cart = new Domain.Entities.Cart
            {
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cartRepository.AddCartAsync(cart);
            await _cartRepository.SaveChangesAsync();
        }
        else if (cart.RestaurantId != request.RestaurantId)
        {
            // Single-restaurant ordering rule: cannot mix items from different restaurants.
            throw new InvalidOperationException(
                "Cart already contains items from a different restaurant. " +
                "Clear the cart or call ChangeRestaurant before adding items from a new restaurant.");
        }

        // If the same menu item (with the same customisation) already exists, bump quantity;
        // otherwise add a new line with a fresh price snapshot.
        var existing = cart.CartItems.FirstOrDefault(x =>
            x.MenuItemId == request.MenuItemId &&
            string.Equals(x.Customization ?? string.Empty, request.Customization ?? string.Empty, StringComparison.Ordinal));

        if (existing is not null)
        {
            existing.Quantity += request.Quantity;
            _cartRepository.UpdateCartItem(existing);
        }
        else
        {
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                MenuItemId = request.MenuItemId,
                Name = request.Name,
                Price = request.Price,
                Quantity = request.Quantity,
                Customization = request.Customization
            };
            await _cartRepository.AddCartItemAsync(cartItem);
            cart.CartItems.Add(cartItem);
        }

        RecomputeTotal(cart);
        cart.UpdatedAt = DateTime.UtcNow;
        _cartRepository.UpdateCart(cart);
        await _cartRepository.SaveChangesAsync();

        var refreshed = await _cartRepository.FindByCustomerIdAsync(request.CustomerId);
        return MapCartToResponse(refreshed!);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveItemAsync(Guid cartItemId)
    {
        var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
        if (cartItem is null) return false;

        var cart = await _cartRepository.FindByCartIdAsync(cartItem.CartId);

        _cartRepository.RemoveCartItem(cartItem);
        await _cartRepository.SaveChangesAsync();

        if (cart is not null)
        {
            // Reload cart so the removed item is gone from the tracked collection.
            cart = await _cartRepository.FindByCartIdAsync(cart.Id);
            if (cart is not null)
            {
                RecomputeTotal(cart);
                cart.UpdatedAt = DateTime.UtcNow;
                _cartRepository.UpdateCart(cart);
                await _cartRepository.SaveChangesAsync();
            }
        }

        return true;
    }

    /// <inheritdoc />
    public async Task<CartResponseDto?> UpdateQuantityAsync(Guid cartItemId, UpdateCartItemRequestDto request)
    {
        var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
        if (cartItem is null) return null;

        cartItem.Quantity = request.Quantity;
        _cartRepository.UpdateCartItem(cartItem);

        var cart = await _cartRepository.FindByCartIdAsync(cartItem.CartId);
        if (cart is not null)
        {
            // The tracked cartItem inside cart.CartItems is the same instance, so the quantity
            // change is already reflected and RecomputeTotal will see the new value.
            RecomputeTotal(cart);
            cart.UpdatedAt = DateTime.UtcNow;
            _cartRepository.UpdateCart(cart);
        }

        await _cartRepository.SaveChangesAsync();

        var refreshed = cart is null ? null : await _cartRepository.FindByCartIdAsync(cart.Id);
        return refreshed is null ? null : MapCartToResponse(refreshed);
    }

    /// <inheritdoc />
    public async Task<bool> ClearCartAsync(Guid customerId)
    {
        var cart = await _cartRepository.FindByCustomerIdAsync(customerId);
        if (cart is null) return false;

        _cartRepository.ClearItems(cart);
        cart.CartItems.Clear();
        cart.TotalPrice = 0m;
        cart.UpdatedAt = DateTime.UtcNow;
        _cartRepository.UpdateCart(cart);

        await _cartRepository.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc />
    public async Task<decimal> CartTotalAsync(Guid customerId)
    {
        var cart = await _cartRepository.FindByCustomerIdAsync(customerId);
        if (cart is null) return 0m;

        return cart.CartItems.Sum(i => i.Price * i.Quantity);
    }

    /// <inheritdoc />
    public async Task<CartResponseDto> ChangeRestaurantAsync(ChangeRestaurantRequestDto request)
    {
        var cart = await _cartRepository.FindByCustomerIdAsync(request.CustomerId);

        if (cart is null)
        {
            // No existing cart: create a fresh one bound to the new restaurant.
            cart = new Domain.Entities.Cart
            {
                CustomerId = request.CustomerId,
                RestaurantId = request.NewRestaurantId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _cartRepository.AddCartAsync(cart);
            await _cartRepository.SaveChangesAsync();
            return MapCartToResponse(cart);
        }

        // Clear all existing items because single-restaurant ordering is enforced.
        _cartRepository.ClearItems(cart);
        cart.CartItems.Clear();
        cart.RestaurantId = request.NewRestaurantId;
        cart.TotalPrice = 0m;
        cart.UpdatedAt = DateTime.UtcNow;
        _cartRepository.UpdateCart(cart);

        await _cartRepository.SaveChangesAsync();
        return MapCartToResponse(cart);
    }

    /// <inheritdoc />
    public async Task<CartResponseDto?> ApplyPromoCodeAsync(ApplyPromoCodeRequestDto request)
    {
        var cart = await _cartRepository.FindByCustomerIdAsync(request.CustomerId);
        if (cart is null) return null;

        // Placeholder promo engine: real discount lookup would live in a dedicated
        // promo service. For now we accept a flat 10% off for any non-empty code.
        var subtotal = cart.CartItems.Sum(i => i.Price * i.Quantity);
        decimal discounted = string.IsNullOrWhiteSpace(request.PromoCode)
            ? subtotal
            : Math.Round(subtotal * 0.9m, 2);

        cart.TotalPrice = discounted;
        cart.UpdatedAt = DateTime.UtcNow;
        _cartRepository.UpdateCart(cart);
        await _cartRepository.SaveChangesAsync();

        return MapCartToResponse(cart);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CartResponseDto>> GetAllCartsAsync()
    {
        var carts = await _cartRepository.GetAllAsync();
        return carts.Select(MapCartToResponse).ToList();
    }

    /// <summary>
    /// Recomputes TotalPrice from the current CartItems collection.
    /// </summary>
    private static void RecomputeTotal(Domain.Entities.Cart cart)
    {
        cart.TotalPrice = cart.CartItems.Sum(i => i.Price * i.Quantity);
    }

    /// <summary>
    /// Converts a Cart entity (with items) into a response DTO.
    /// </summary>
    private static CartResponseDto MapCartToResponse(Domain.Entities.Cart cart)
    {
        var items = cart.CartItems.Select(item => new CartItemResponseDto
        {
            ItemId = item.Id,
            MenuItemId = item.MenuItemId,
            Name = item.Name,
            Price = item.Price,
            Quantity = item.Quantity,
            Customization = item.Customization,
            LineTotal = item.Price * item.Quantity
        }).ToList();

        return new CartResponseDto
        {
            CartId = cart.Id,
            CustomerId = cart.CustomerId,
            RestaurantId = cart.RestaurantId,
            Items = items,
            TotalPrice = cart.TotalPrice
        };
    }
}
