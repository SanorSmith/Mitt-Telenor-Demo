using UserService.Models;

namespace UserService.Services;

public interface IUserService
{
    Task<ProfileResponse?> GetProfileByUserIdAsync(Guid userId);
    Task<ProfileResponse> CreateProfileAsync(Guid userId, string email, string firstName, string lastName, string? phone);
    Task<ProfileResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<bool> DeleteProfileAsync(Guid userId);
    Task<List<NotificationDto>> GetNotificationsAsync(Guid userId, bool unreadOnly = false);
    Task<bool> MarkNotificationAsReadAsync(Guid notificationId, Guid userId);
}
