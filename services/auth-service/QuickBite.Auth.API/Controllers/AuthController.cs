using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickBite.Auth.Application.DTOs;
using QuickBite.Auth.Application.Interfaces;
using QuickBite.Auth.Domain.Enums;
using System.Security.Claims;

namespace QuickBite.Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var user = new UserResponseDto
            {
                UserId = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
                    ? userId
                    : Guid.Empty,
                FullName = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                Phone = string.Empty,
                Role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty
            };

            return Ok(new
            {
                message = "Authenticated user details fetched successfully",
                user
            });
        }

        [Authorize]
        [HttpGet("validate-token")]
        public IActionResult ValidateToken()
        {
            return Ok(new
            {
                message = "Token is valid",
                isAuthenticated = User.Identity?.IsAuthenticated ?? false,
                role = User.FindFirstValue(ClaimTypes.Role)
            });
        }

        [Authorize(Roles = UserRole.Admin)]
        [HttpGet("admin-only")]
        public IActionResult AdminOnly()
        {
            return Ok(new
            {
                message = "Welcome Admin! You are authorized to access this endpoint."
            });
        }

        [Authorize(Roles = UserRole.Customer)]
        [HttpGet("customer-only")]
        public IActionResult CustomerOnly()
        {
            return Ok(new
            {
                message = "Welcome Customer! You are authorized to access this endpoint."
            });
        }

        [Authorize(Roles = UserRole.Admin + "," + UserRole.RestaurantOwner)]
        [HttpGet("admin-or-owner")]
        public IActionResult AdminOrOwner()
        {
            return Ok(new
            {
                message = "Welcome! This endpoint can be accessed by Admin or Restaurant Owner."
            });
        }
    }
}
