using QuickBite.Auth.Application.DTOs;

namespace QuickBite.Auth.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);

        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    }
}