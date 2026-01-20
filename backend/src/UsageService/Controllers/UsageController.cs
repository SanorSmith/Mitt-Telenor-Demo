using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsageService.Models;
using UsageService.Services;

namespace UsageService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsageController : ControllerBase
{
    private readonly IUsageService _usageService;
    private readonly IUsageTrackingService _trackingService;
    private readonly ILogger<UsageController> _logger;

    public UsageController(
        IUsageService usageService,
        IUsageTrackingService trackingService,
        ILogger<UsageController> logger)
    {
        _usageService = usageService;
        _trackingService = trackingService;
        _logger = logger;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    [HttpGet("current")]
    public async Task<ActionResult<UsageSummary>> GetCurrentUsage()
    {
        var userId = GetUserId();
        var usage = await _usageService.GetCurrentUsageAsync(userId);
        return Ok(usage);
    }

    [HttpGet("daily")]
    public async Task<ActionResult<List<DailyUsage>>> GetDailyUsage([FromQuery] int days = 30)
    {
        if (days < 1 || days > 90)
            return BadRequest(new { message = "Days must be between 1 and 90" });

        var userId = GetUserId();
        var usage = await _usageService.GetDailyUsageAsync(userId, days);
        return Ok(usage);
    }

    [HttpGet("history")]
    public async Task<ActionResult<UsageHistoryResponse>> GetUsageHistory([FromQuery] UsageHistoryRequest request)
    {
        var userId = GetUserId();
        var history = await _usageService.GetUsageHistoryAsync(userId, request);
        return Ok(history);
    }

    [HttpPost("record")]
    public async Task<ActionResult> RecordUsage([FromBody] RecordUsageRequest request)
    {
        var userId = GetUserId();
        await _trackingService.RecordUsageAsync(userId, request);
        return Ok(new { message = "Usage recorded successfully" });
    }
}
