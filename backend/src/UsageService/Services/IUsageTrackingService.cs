using UsageService.Models;

namespace UsageService.Services;

public interface IUsageTrackingService
{
    Task RecordUsageAsync(Guid userId, RecordUsageRequest request);
}
