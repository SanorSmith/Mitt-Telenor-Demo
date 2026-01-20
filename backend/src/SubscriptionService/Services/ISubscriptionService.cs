using SubscriptionService.Models;

namespace SubscriptionService.Services;

public interface ISubscriptionService
{
    Task<SubscriptionDto?> GetUserSubscriptionAsync(Guid userId);
    Task<SubscriptionDto> CreateSubscriptionAsync(Guid userId, CreateSubscriptionRequest request);
    Task<SubscriptionDto> UpdateSubscriptionAsync(Guid userId, UpdateSubscriptionRequest request);
    Task<bool> CancelSubscriptionAsync(Guid userId);
    Task<SubscriptionDto> AddAddOnAsync(Guid userId, Guid addOnId);
    Task<bool> RemoveAddOnAsync(Guid userId, Guid addOnId);
}
