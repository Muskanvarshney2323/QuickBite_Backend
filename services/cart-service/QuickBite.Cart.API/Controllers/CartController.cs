// Used for authorization features like [Authorize] attribute
using Microsoft.AspNetCore.Authorization;

// Used for Web API controller features, routing, IActionResult, etc.
using Microsoft.AspNetCore.Mvc;

// DTO (Data Transfer Object) classes for request/response
using QuickBite.Cart.Application.DTOs;

// Service interface for business logic
using QuickBite.Cart.Application.Interfaces;

// Used for Swagger/OpenAPI documentation
using Swashbuckle.AspNetCore.Annotations;

// Namespace for this controller
namespace QuickBite.Cart.API.Controllers;

// ========================= CART CONTROLLER SUMMARY =========================
/// <summary>
/// CartController: Exposes HTTP endpoints for shopping cart operations
/// Enforces single-restaurant ordering via service layer
/// Endpoints: get/clear cart, add/remove/update items, apply promo, change restaurant
/// </summary>

// ========================= ATTRIBUTES =========================

// Mark this class as an API Controller (enables automatic model validation)
[ApiController]

// Base route for all endpoints in this controller
// Example: GET /api/v1/cart/{customerId}
[Route("api/v1/cart")]

// Requires JWT authentication token for all endpoints
[Authorize]

// Swagger documentation tag
[SwaggerTag("Cart operations: get by customer, add/update/remove items, clear, apply promo, change restaurant, get all")]
public class CartController : ControllerBase
{
    // Service object that contains all cart business logic
    private readonly ICartService _cartService;

    // ========================= CONSTRUCTOR =========================

    // Constructor with Dependency Injection
    // ASP.NET automatically injects ICartService from DI container
    public CartController(ICartService cartService)
    {
        // Store service reference for use in endpoint methods
        _cartService = cartService;
    }

    /// <summary>  fetch a customer's cart.</summary>
    [HttpGet("{customerId:guid}")]
    [SwaggerOperation(Summary = "Get cart by customer", Description = "Returns the active cart for a customer")]
    [SwaggerResponse(200, "Cart fetched successfully", typeof(CartResponseDto))]
    [SwaggerResponse(404, "Cart not found")]
    public async Task<IActionResult> GetCartByCustomer(Guid customerId)
    {
        var result = await _cartService.GetCartByCustomerAsync(customerId);
        return result is null ? NotFound("Cart not found.") : Ok(result);
    }

    /// <summary> fetch all carts (admin / diagnostics).</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all carts", Description = "Returns every active cart in the system")]
    [SwaggerResponse(200, "Carts fetched successfully", typeof(IReadOnlyList<CartResponseDto>))]
    public async Task<IActionResult> GetAllCarts()
    {
        var result = await _cartService.GetAllCartsAsync();
        return Ok(result);
    }

    /// <summary> add an item to the cart.</summary>
    [HttpPost("addItem")]
    [SwaggerOperation(Summary = "Add item", Description = "Adds an item to the customer's cart (creates the cart on first add). Rejects the add if the item's restaurant differs from the cart's restaurant.")]
    [SwaggerResponse(200, "Item added successfully", typeof(CartResponseDto))]
    [SwaggerResponse(400, "Invalid request or restaurant mismatch")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemRequestDto request)
    {
        if (request.Quantity <= 0)
            return BadRequest("Quantity must be greater than 0.");

        if (request.Price < 0)
            return BadRequest("Price cannot be negative.");

        if (request.CustomerId == Guid.Empty)
            return BadRequest("CustomerId is required.");

        if (request.RestaurantId == Guid.Empty)
            return BadRequest("RestaurantId is required.");

        try
        {
            var result = await _cartService.AddItemAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            // Raised when the item's restaurant doesn't match the cart's restaurant.
            return BadRequest(ex.Message);
        }
    }

    /// <summary> update the quantity of a cart item.</summary>
    [HttpPut("updateQty/{cartItemId:guid}")]
    [SwaggerOperation(Summary = "Update quantity", Description = "Updates the quantity of a specific cart item")]
    [SwaggerResponse(200, "Quantity updated successfully", typeof(CartResponseDto))]
    [SwaggerResponse(400, "Invalid quantity")]
    [SwaggerResponse(404, "Cart item not found")]
    public async Task<IActionResult> UpdateQuantity(Guid cartItemId, [FromBody] UpdateCartItemRequestDto request)
    {
        if (request.Quantity <= 0)
            return BadRequest("Quantity must be greater than 0.");

        var result = await _cartService.UpdateQuantityAsync(cartItemId, request);
        return result is null ? NotFound("Cart item not found.") : Ok(result);
    }

    /// <summary> remove a single item.</summary>
    [HttpDelete("removeItem/{cartItemId:guid}")]
    [SwaggerOperation(Summary = "Remove item", Description = "Removes a specific item from the cart")]
    [SwaggerResponse(200, "Item removed successfully")]
    [SwaggerResponse(404, "Cart item not found")]
    public async Task<IActionResult> RemoveItem(Guid cartItemId)
    {
        var removed = await _cartService.RemoveItemAsync(cartItemId);
        return removed ? Ok("Cart item removed successfully.") : NotFound("Cart item not found.");
    }

    /// <summary> remove everything from the customer's cart.</summary>
    [HttpDelete("clear/{customerId:guid}")]
    [SwaggerOperation(Summary = "Clear cart", Description = "Removes every item from the customer's cart")]
    [SwaggerResponse(200, "Cart cleared successfully")]
    [SwaggerResponse(404, "Cart not found")]
    public async Task<IActionResult> ClearCart(Guid customerId)
    {
        var cleared = await _cartService.ClearCartAsync(customerId);
        return cleared ? Ok("Cart cleared successfully.") : NotFound("Cart not found.");
    }

    /// <summary> compute current cart total.</summary>
    [HttpGet("total/{customerId:guid}")]
    [SwaggerOperation(Summary = "Cart total", Description = "Returns the current total price of the customer's cart")]
    [SwaggerResponse(200, "Total computed successfully")]
    public async Task<IActionResult> CartTotal(Guid customerId)
    {
        var total = await _cartService.CartTotalAsync(customerId);
        return Ok(new { customerId, totalPrice = total });
    }

    /// <summary>POST  switch the cart to a different restaurant (clears items).</summary>
    [HttpPost("changeRestaurant")]
    [SwaggerOperation(Summary = "Change restaurant", Description = "Switches the customer's cart to a new restaurant. Clears existing items because only one restaurant is allowed per cart.")]
    [SwaggerResponse(200, "Restaurant changed successfully", typeof(CartResponseDto))]
    [SwaggerResponse(400, "Invalid request")]
    public async Task<IActionResult> ChangeRestaurant([FromBody] ChangeRestaurantRequestDto request)
    {
        if (request.CustomerId == Guid.Empty)
            return BadRequest("CustomerId is required.");

        if (request.NewRestaurantId == Guid.Empty)
            return BadRequest("NewRestaurantId is required.");

        var result = await _cartService.ChangeRestaurantAsync(request);
        return Ok(result);
    }

    /// <summary> apply a promo code to the customer's cart.</summary>
    [HttpPost("applyPromo")]
    [SwaggerOperation(Summary = "Apply promo code", Description = "Applies a promo code to the customer's cart and returns the updated cart")]
    [SwaggerResponse(200, "Promo applied successfully", typeof(CartResponseDto))]
    [SwaggerResponse(404, "Cart not found")]
    public async Task<IActionResult> ApplyPromo([FromBody] ApplyPromoCodeRequestDto request)
    {
        if (request.CustomerId == Guid.Empty)
            return BadRequest("CustomerId is required.");

        var result = await _cartService.ApplyPromoCodeAsync(request);
        return result is null ? NotFound("Cart not found.") : Ok(result);
    }
}
