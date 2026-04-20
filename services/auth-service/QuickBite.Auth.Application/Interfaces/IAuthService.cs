using QuickBite.Auth.Application.DTOs;
using QuickBite.Auth.Domain.Entities;

namespace QuickBite.Auth.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterRequestDto request);
        Task<string> LoginAsync(LoginRequestDto request);
        Task<User?> GetCurrentUserAsync(string email);
        Task<bool> ValidateTokenUserAsync(string email);
    }
}