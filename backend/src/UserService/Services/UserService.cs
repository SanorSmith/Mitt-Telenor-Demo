using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using UserService.Data;
using UserService.Models;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly UserDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<UserService> _logger;

    public UserService(UserDbContext context, IDistributedCache cache, ILogger<UserService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ProfileResponse?> GetProfileByUserIdAsync(Guid userId)
    {
        var cacheKey = $"profile:{userId}";
        var cachedProfile = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedProfile))
        {
            return JsonSerializer.Deserialize<ProfileResponse>(cachedProfile);
        }

        var profile = await _context.UserProfiles
            .Include(p => p.Addresses)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null) return null;

        var response = MapToProfileResponse(profile);

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            });

        return response;
    }

    public async Task<ProfileResponse> CreateProfileAsync(Guid userId, string email, string firstName, string lastName, string? phone)
    {
        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created profile for user {UserId}", userId);

        return MapToProfileResponse(profile);
    }

    public async Task<ProfileResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var profile = await _context.UserProfiles
            .Include(p => p.Addresses)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
            throw new KeyNotFoundException($"Profile not found for user {userId}");

        profile.FirstName = request.FirstName;
        profile.LastName = request.LastName;
        profile.Phone = request.Phone;
        profile.DateOfBirth = request.DateOfBirth;
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _cache.RemoveAsync($"profile:{userId}");

        _logger.LogInformation("Updated profile for user {UserId}", userId);

        return MapToProfileResponse(profile);
    }

    public async Task<bool> DeleteProfileAsync(Guid userId)
    {
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null) return false;

        _context.UserProfiles.Remove(profile);
        await _context.SaveChangesAsync();

        await _cache.RemoveAsync($"profile:{userId}");

        _logger.LogInformation("Deleted profile for user {UserId}", userId);

        return true;
    }

    public async Task<List<NotificationDto>> GetNotificationsAsync(Guid userId, bool unreadOnly = false)
    {
        var profile = await _context.UserProfiles
            .Include(p => p.Notifications)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null) return new List<NotificationDto>();

        var notifications = profile.Notifications.AsQueryable();

        if (unreadOnly)
            notifications = notifications.Where(n => !n.IsRead);

        return notifications
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Type = n.Type,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            })
            .ToList();
    }

    public async Task<bool> MarkNotificationAsReadAsync(Guid notificationId, Guid userId)
    {
        var profile = await _context.UserProfiles
            .Include(p => p.Notifications)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null) return false;

        var notification = profile.Notifications.FirstOrDefault(n => n.Id == notificationId);
        if (notification == null) return false;

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return true;
    }

    private static ProfileResponse MapToProfileResponse(UserProfile profile)
    {
        return new ProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            Email = profile.Email,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Phone = profile.Phone,
            DateOfBirth = profile.DateOfBirth,
            ProfileImageUrl = profile.ProfileImageUrl,
            Addresses = profile.Addresses.Select(a => new AddressDto
            {
                Id = a.Id,
                Street = a.Street,
                City = a.City,
                PostalCode = a.PostalCode,
                Country = a.Country,
                IsPrimary = a.IsPrimary
            }).ToList(),
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };
    }
}
