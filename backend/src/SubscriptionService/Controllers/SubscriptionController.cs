using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SubscriptionService.Models;
using SubscriptionService.Services;

namespace SubscriptionService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IPlanService _planService;
    private readonly ILogger<SubscriptionController> _logger;

    public SubscriptionController(
        ISubscriptionService subscriptionService,
        IPlanService planService,
        ILogger<SubscriptionController> logger)
    {
        _subscriptionService = subscriptionService;
        _planService = planService;
        _logger = logger;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    [HttpGet("plans")]
    [AllowAnonymous]
    public async Task<ActionResult<List<PlanDto>>> GetPlans()
    {
        var plans = await _planService.GetAllPlansAsync();
        return Ok(plans);
    }

    [HttpGet("addons")]
    [AllowAnonymous]
    public async Task<ActionResult<List<AddOnDto>>> GetAddOns()
    {
        var addOns = await _planService.GetAllAddOnsAsync();
        return Ok(addOns);
    }

    [HttpGet("my-subscription")]
    public async Task<ActionResult<SubscriptionDto>> GetMySubscription()
    {
        var userId = GetUserId();
        var subscription = await _subscriptionService.GetUserSubscriptionAsync(userId);

        if (subscription == null)
            return NotFound(new { message = "No active subscription found" });

        return Ok(subscription);
    }

    [HttpPost]
    public async Task<ActionResult<SubscriptionDto>> CreateSubscription([FromBody] CreateSubscriptionRequest request)
    {
        var userId = GetUserId();

        try
        {
            var subscription = await _subscriptionService.CreateSubscriptionAsync(userId, request);
            return CreatedAtAction(nameof(GetMySubscription), new { }, subscription);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<ActionResult<SubscriptionDto>> UpdateSubscription([FromBody] UpdateSubscriptionRequest request)
    {
        var userId = GetUserId();

        try
        {
            var subscription = await _subscriptionService.UpdateSubscriptionAsync(userId, request);
            return Ok(subscription);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete]
    public async Task<ActionResult> CancelSubscription()
    {
        var userId = GetUserId();
        var success = await _subscriptionService.CancelSubscriptionAsync(userId);

        if (!success)
            return NotFound(new { message = "No active subscription found" });

        return NoContent();
    }

    [HttpPost("addons/{addOnId}")]
    public async Task<ActionResult<SubscriptionDto>> AddAddOn(Guid addOnId)
    {
        var userId = GetUserId();

        try
        {
            var subscription = await _subscriptionService.AddAddOnAsync(userId, addOnId);
            return Ok(subscription);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("addons/{addOnId}")]
    public async Task<ActionResult> RemoveAddOn(Guid addOnId)
    {
        var userId = GetUserId();
        var success = await _subscriptionService.RemoveAddOnAsync(userId, addOnId);

        if (!success)
            return NotFound(new { message = "Add-on not found in subscription" });

        return NoContent();
    }
}
