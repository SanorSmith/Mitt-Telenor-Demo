using System.Security.Claims;

namespace AuthService.Services;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, string role);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}
