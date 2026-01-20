using AuthService.Models;

namespace AuthService.Services;

public interface IAuthService
{
    Task<TokenResponse> RegisterAsync(RegisterRequest request);
    Task<TokenResponse> LoginAsync(LoginRequest request);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string token);
}
