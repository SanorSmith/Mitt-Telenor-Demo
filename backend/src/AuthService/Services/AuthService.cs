using AuthService.Data;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace AuthService.Services;

public class AuthService : IAuthService
{
    private readonly AuthDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AuthDbContext context,
        ITokenService tokenService,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<TokenResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Role = "Customer",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User registered successfully: {Email}", user.Email);

        return await GenerateTokenResponseAsync(user);
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User logged in successfully: {Email}", user.Email);

        return await GenerateTokenResponseAsync(user);
    }

    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);

        if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var user = await _context.Users.FindAsync(storedToken.UserId);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("User not found or inactive");
        }

        storedToken.IsRevoked = true;
        await _context.SaveChangesAsync();

        return await GenerateTokenResponseAsync(user);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null)
        {
            return false;
        }

        storedToken.IsRevoked = true;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Refresh token revoked for user: {UserId}", storedToken.UserId);

        return true;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var principal = _tokenService.ValidateToken(token);
        if (principal == null)
        {
            return false;
        }

        var userIdClaim = principal.FindFirst("user_id");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return false;
        }

        var user = await _context.Users.FindAsync(userId);
        return user != null && user.IsActive;
    }

    private async Task<TokenResponse> GenerateTokenResponseAsync(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenExpiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7");

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiryDays),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "15");

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Role = user.Role
            }
        };
    }
}
