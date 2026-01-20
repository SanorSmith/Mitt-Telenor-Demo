using UsageService.Models;

namespace UsageService.Services;

public interface IUsageService
{
    Task<UsageSummary> GetCurrentUsageAsync(Guid userId);
    Task<List<DailyUsage>> GetDailyUsageAsync(Guid userId, int days = 30);
    Task<UsageHistoryResponse> GetUsageHistoryAsync(Guid userId, UsageHistoryRequest request);
}
