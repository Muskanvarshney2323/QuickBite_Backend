using QuickBite.Cart.Domain.Entities;

namespace QuickBite.Cart.Application.Interfaces;

/// <summary>
/// Repository contract for cart persistence.
/// Method names mirror the Cart-Service class diagram spec:
/// FindByCustomerId, FindByCartId, ExistsByCustomerId,
/// FindByRestaurantId, DeleteByCustomerId.
/// </summary>
public interface ICartRepository
{
    /// <summary>Find the active cart for a given customer (or null).</summary>
    Task<Domain.Entities.Cart?> FindByCustomerIdAsync(Guid customerId);

    /// <summary>Find a cart by its own id (or null).</summary>
    Task<Domain.Entities.Cart?> FindByCartIdAsync(Guid cartId);

    /// <summary>Whether an active cart exists for the given customer.</summary>
    Task<bool> ExistsByCustomerIdAsync(Guid customerId);

    /// <summary>All active carts bound to a specific restaurant.</summary>
    Task<IReadOnlyList<Domain.Entities.Cart>> FindByRestaurantIdAsync(Guid restaurantId);

    /// <summary>Delete the active cart for a given customer. Returns true if a cart was deleted.</summary>
    Task<bool> DeleteByCustomerIdAsync(Guid customerId);

    /// <summary>All carts in the system (admin / diagnostics).</summary>
    Task<IReadOnlyList<Domain.Entities.Cart>> GetAllAsync();

    /// <summary>Persist a new cart.</summary>
    Task AddCartAsync(Domain.Entities.Cart cart);

    /// <summary>Persist a new cart item.</summary>
    Task AddCartItemAsync(CartItem cartItem);

    /// <summary>Find a cart item by its own id.</summary>
    Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId);

    /// <summary>Mark a cart as updated.</summary>
    void UpdateCart(Domain.Entities.Cart cart);

    /// <summary>Mark a cart item as updated.</summary>
    void UpdateCartItem(CartItem cartItem);

    /// <summary>Remove a single cart item.</summary>
    void RemoveCartItem(CartItem cartItem);

    /// <summary>Remove all items from the given cart (but keep the cart).</summary>
    void ClearItems(Domain.Entities.Cart cart);

    /// <summary>Commit pending changes to the database.</summary>
    Task SaveChangesAsync();
}
