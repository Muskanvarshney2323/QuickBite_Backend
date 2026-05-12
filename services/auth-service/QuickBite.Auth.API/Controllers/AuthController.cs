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
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto? request)
        {
            var email = GetCurrentUserEmail();
            await _authService.LogoutAsync(email);

            return Ok(new
            {
                message = "Logout successful. Please remove the token from client side storage.",
                token = request?.Token
            });
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var email = GetCurrentUserEmail();
            var token = await _authService.RefreshTokenAsync(email);

            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email = GetCurrentUserEmail();
            var user = await _authService.GetCurrentUserAsync(email);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var email = GetCurrentUserEmail();
            var user = await _authService.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request)
        {
            var email = GetCurrentUserEmail();
            var updatedUser = await _authService.UpdateProfileAsync(email, request);
            return Ok(new { message = "Profile updated successfully.", user = updatedUser });
        }

        [Authorize]
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var email = GetCurrentUserEmail();
            await _authService.ChangePasswordAsync(email, request);
            return Ok(new { message = "Password changed successfully." });
        }

        [Authorize]
        [HttpDelete("deactivate")]
        public async Task<IActionResult> DeactivateAccount()
        {
            var email = GetCurrentUserEmail();
            await _authService.DeactivateAccountAsync(email);
            return Ok(new { message = "Account deactivated successfully." });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("users/{userId:guid}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users/by-email")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            var user = await _authService.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        [Authorize]
        [HttpGet("validate-token")]
        public async Task<IActionResult> ValidateToken()
        {
            var email = GetCurrentUserEmail();
            var isValidUser = await _authService.ValidateTokenUserAsync(email);

            if (!isValidUser)
            {
                return Unauthorized(new { message = "User does not exist or account is inactive." });
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

        [Authorize(Roles = "Admin,RestaurantOwner")]
        [HttpGet("admin-or-owner")]
        public IActionResult AdminOrOwner()
        {
            return Ok(new { message = "Welcome Admin or Owner" });
        }

        private string GetCurrentUserEmail()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }

            return email;
        }
    }
}