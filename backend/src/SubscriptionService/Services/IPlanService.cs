using SubscriptionService.Models;

namespace SubscriptionService.Services;

public interface IPlanService
{
    Task<List<PlanDto>> GetAllPlansAsync();
    Task<List<AddOnDto>> GetAllAddOnsAsync();
}
