using QuickBite.Auth.Domain.Entities;

namespace QuickBite.Auth.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}