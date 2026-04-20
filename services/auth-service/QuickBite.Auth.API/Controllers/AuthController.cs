using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using QuickBite.Auth.Application.DTOs;
using QuickBite.Auth.Application.Interfaces;

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
            var result = await _authService.RegisterAsync(request);
            return Ok(new { message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var token = await _authService.LoginAsync(request);
            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
            {
                return Unauthorized(new { message = "Invalid token." });
            }

            var user = await _authService.GetCurrentUserAsync(email);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(new
            {
                user.UserId,
                user.FullName,
                user.Email,
                user.Role
            });
        }

        [Authorize]
        [HttpGet("validate-token")]
        public async Task<IActionResult> ValidateToken()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
            {
                return Unauthorized(new { message = "Invalid token." });
            }

            var isValidUser = await _authService.ValidateTokenUserAsync(email);

            if (!isValidUser)
            {
                return Unauthorized(new { message = "User does not exist." });
            }

            return Ok(new { message = "Token is valid." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnly()
        {
            return Ok(new { message = "Welcome Admin" });
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("customer-only")]
        public IActionResult CustomerOnly()
        {
            return Ok(new { message = "Welcome Customer" });
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpGet("admin-or-owner")]
        public IActionResult AdminOrOwner()
        {
            return Ok(new { message = "Welcome Admin or Owner" });
        }
    }
}