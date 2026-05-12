using QuickBite.Auth.Application.DTOs;

namespace QuickBite.Auth.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterRequestDto request);
        Task<string> LoginAsync(LoginRequestDto request);
        Task<UserResponseDto?> GetCurrentUserAsync(string email);
        Task<bool> ValidateTokenUserAsync(string email);
        Task<UserResponseDto?> GetUserByIdAsync(Guid userId);
        Task<UserResponseDto?> GetUserByEmailAsync(string email);
        Task<UserResponseDto> UpdateProfileAsync(string email, UpdateProfileRequestDto request);
        Task ChangePasswordAsync(string email, ChangePasswordRequestDto request);
        Task DeactivateAccountAsync(string email);
        Task<string> RefreshTokenAsync(string email);
        Task LogoutAsync(string email);
    }
}
