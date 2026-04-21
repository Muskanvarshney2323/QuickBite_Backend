using Microsoft.AspNetCore.Mvc;
using QuickBite.Cart.Application.DTOs;
using QuickBite.Cart.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace QuickBite.Cart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Cart operations: Add, Update, Remove, Clear cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>
    /// Adds an item to the user's cart
    /// </summary>
    [HttpPost("add-item")]
    [SwaggerOperation(
        Summary = "Add item to cart",
        Description = "Adds a new item or increases quantity if item already exists in cart"
    )]
    [SwaggerResponse(200, "Item added successfully", typeof(CartResponseDto))]
    [SwaggerResponse(400, "Invalid request data")]
    public async Task<IActionResult> AddItemToCart([FromBody] AddCartItemRequestDto request)
    {
        if (request.Quantity <= 0)
            return BadRequest("Quantity must be greater than 0.");

        if (request.UnitPrice < 0)
            return BadRequest("Unit price cannot be negative.");

        var result = await _cartService.AddItemToCartAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Gets cart details for a user
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [SwaggerOperation(
        Summary = "Get cart by user",
        Description = "Fetches all items in the user's cart"
    )]
    [SwaggerResponse(200, "Cart fetched successfully", typeof(CartResponseDto))]
    [SwaggerResponse(404, "Cart not found")]
    public async Task<IActionResult> GetCartByUserId(Guid userId)
    {
        var result = await _cartService.GetCartByUserIdAsync(userId);

        if (result == null)
            return NotFound("Cart not found.");

        return Ok(result);
    }

    /// <summary>
    /// Updates quantity of a cart item
    /// </summary>
    [HttpPut("item/{cartItemId:guid}")]
    [SwaggerOperation(
        Summary = "Update cart item",
        Description = "Updates the quantity of a specific cart item"
    )]
    [SwaggerResponse(200, "Item updated successfully", typeof(CartResponseDto))]
    [SwaggerResponse(400, "Invalid quantity")]
    [SwaggerResponse(404, "Cart item not found")]
    public async Task<IActionResult> UpdateCartItem(Guid cartItemId, [FromBody] UpdateCartItemRequestDto request)
    {
        if (request.Quantity <= 0)
            return BadRequest("Quantity must be greater than 0.");

        var result = await _cartService.UpdateCartItemAsync(cartItemId, request);

        if (result == null)
            return NotFound("Cart item not found.");

        return Ok(result);
    }

    /// <summary>
    /// Removes an item from the cart
    /// </summary>
    [HttpDelete("item/{cartItemId:guid}")]
    [SwaggerOperation(
        Summary = "Remove cart item",
        Description = "Deletes a specific item from the cart"
    )]
    [SwaggerResponse(200, "Item removed successfully")]
    [SwaggerResponse(404, "Cart item not found")]
    public async Task<IActionResult> RemoveCartItem(Guid cartItemId)
    {
        var removed = await _cartService.RemoveCartItemAsync(cartItemId);

        if (!removed)
            return NotFound("Cart item not found.");

        return Ok("Cart item removed successfully.");
    }

    /// <summary>
    /// Clears entire cart
    /// </summary>
    [HttpDelete("clear/{userId:guid}")]
    [SwaggerOperation(
        Summary = "Clear cart",
        Description = "Removes all items from the user's cart"
    )]
    [SwaggerResponse(200, "Cart cleared successfully")]
    [SwaggerResponse(404, "Cart not found")]
    public async Task<IActionResult> ClearCart(Guid userId)
    {
        var cleared = await _cartService.ClearCartAsync(userId);

        if (!cleared)
            return NotFound("Cart not found.");

        return Ok("Cart cleared successfully.");
    }
}