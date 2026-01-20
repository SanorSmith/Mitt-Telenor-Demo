using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using SubscriptionService.Data;
using SubscriptionService.Models;
using Shared.Messaging;
using Shared.Events;

namespace SubscriptionService.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly SubscriptionDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly IEventPublisher _eventPublisher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(
        SubscriptionDbContext context,
        IDistributedCache cache,
        IEventPublisher eventPublisher,
        IConfiguration configuration,
        ILogger<SubscriptionService> logger)
    {
        _context = context;
        _cache = cache;
        _eventPublisher = eventPublisher;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<SubscriptionDto?> GetUserSubscriptionAsync(Guid userId)
    {
        var cacheKey = $"subscription:{userId}";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<SubscriptionDto>(cached);
        }

        var subscription = await _context.Subscriptions
            .Include(s => s.Plan)
            .Include(s => s.SubscriptionAddOns)
                .ThenInclude(sa => sa.AddOn)
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "Active");

        if (subscription == null) return null;

        var dto = MapToDto(subscription);

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            });

        return dto;
    }

    public async Task<SubscriptionDto> CreateSubscriptionAsync(Guid userId, CreateSubscriptionRequest request)
    {
        var existingSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "Active");

        if (existingSubscription != null)
            throw new InvalidOperationException("User already has an active subscription");

        var plan = await _context.Plans.FindAsync(request.PlanId);
        if (plan == null || !plan.IsActive)
            throw new KeyNotFoundException("Plan not found or inactive");

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PlanId = request.PlanId,
            Status = "Active",
            StartDate = DateTime.UtcNow,
            BillingCycle = request.BillingCycle,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Subscriptions.Add(subscription);

        foreach (var addOnId in request.AddOnIds)
        {
            var addOn = await _context.AddOns.FindAsync(addOnId);
            if (addOn != null && addOn.IsActive)
            {
                _context.SubscriptionAddOns.Add(new SubscriptionAddOn
                {
                    Id = Guid.NewGuid(),
                    SubscriptionId = subscription.Id,
                    AddOnId = addOnId,
                    AddedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();

        await _cache.RemoveAsync($"subscription:{userId}");

        await _eventPublisher.PublishAsync(
            _configuration["AWS:SnsTopicArn"]!,
            new SubscriptionChangedEvent
            {
                UserId = userId,
                SubscriptionId = subscription.Id,
                PlanId = request.PlanId,
                ChangeType = "Created",
                Timestamp = DateTime.UtcNow
            });

        _logger.LogInformation("Created subscription for user {UserId}", userId);

        subscription.Plan = plan;
        return MapToDto(subscription);
    }

    public async Task<SubscriptionDto> UpdateSubscriptionAsync(Guid userId, UpdateSubscriptionRequest request)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Plan)
            .Include(s => s.SubscriptionAddOns)
                .ThenInclude(sa => sa.AddOn)
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "Active");

        if (subscription == null)
            throw new KeyNotFoundException("Active subscription not found");

        var newPlan = await _context.Plans.FindAsync(request.PlanId);
        if (newPlan == null || !newPlan.IsActive)
            throw new KeyNotFoundException("Plan not found or inactive");

        var oldPlanId = subscription.PlanId;
        subscription.PlanId = request.PlanId;
        subscription.Plan = newPlan;
        subscription.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _cache.RemoveAsync($"subscription:{userId}");

        await _eventPublisher.PublishAsync(
            _configuration["AWS:SnsTopicArn"]!,
            new SubscriptionChangedEvent
            {
                UserId = userId,
                SubscriptionId = subscription.Id,
                PlanId = request.PlanId,
                ChangeType = "Updated",
                Timestamp = DateTime.UtcNow
            });

        _logger.LogInformation("Updated subscription for user {UserId} from plan {OldPlanId} to {NewPlanId}",
            userId, oldPlanId, request.PlanId);

        return MapToDto(subscription);
    }

    public async Task<bool> CancelSubscriptionAsync(Guid userId)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "Active");

        if (subscription == null) return false;

        subscription.Status = "Cancelled";
        subscription.EndDate = DateTime.UtcNow;
        subscription.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _cache.RemoveAsync($"subscription:{userId}");

        await _eventPublisher.PublishAsync(
            _configuration["AWS:SnsTopicArn"]!,
            new SubscriptionChangedEvent
            {
                UserId = userId,
                SubscriptionId = subscription.Id,
                PlanId = subscription.PlanId,
                ChangeType = "Cancelled",
                Timestamp = DateTime.UtcNow
            });

        _logger.LogInformation("Cancelled subscription for user {UserId}", userId);

        return true;
    }

    public async Task<SubscriptionDto> AddAddOnAsync(Guid userId, Guid addOnId)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Plan)
            .Include(s => s.SubscriptionAddOns)
                .ThenInclude(sa => sa.AddOn)
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "Active");

        if (subscription == null)
            throw new KeyNotFoundException("Active subscription not found");

        var addOn = await _context.AddOns.FindAsync(addOnId);
        if (addOn == null || !addOn.IsActive)
            throw new KeyNotFoundException("Add-on not found or inactive");

        if (subscription.SubscriptionAddOns.Any(sa => sa.AddOnId == addOnId))
            throw new InvalidOperationException("Add-on already added to subscription");

        _context.SubscriptionAddOns.Add(new SubscriptionAddOn
        {
            Id = Guid.NewGuid(),
            SubscriptionId = subscription.Id,
            AddOnId = addOnId,
            AddedAt = DateTime.UtcNow
        });

        subscription.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _cache.RemoveAsync($"subscription:{userId}");

        _logger.LogInformation("Added add-on {AddOnId} to subscription for user {UserId}", addOnId, userId);

        return MapToDto(subscription);
    }

    public async Task<bool> RemoveAddOnAsync(Guid userId, Guid addOnId)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.SubscriptionAddOns)
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "Active");

        if (subscription == null) return false;

        var subscriptionAddOn = subscription.SubscriptionAddOns.FirstOrDefault(sa => sa.AddOnId == addOnId);
        if (subscriptionAddOn == null) return false;

        _context.SubscriptionAddOns.Remove(subscriptionAddOn);
        subscription.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _cache.RemoveAsync($"subscription:{userId}");

        _logger.LogInformation("Removed add-on {AddOnId} from subscription for user {UserId}", addOnId, userId);

        return true;
    }

    private static SubscriptionDto MapToDto(Subscription subscription)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            Plan = new PlanDto
            {
                Id = subscription.Plan.Id,
                Name = subscription.Plan.Name,
                Description = subscription.Plan.Description,
                Price = subscription.Plan.Price,
                DataAllowanceGB = subscription.Plan.DataAllowanceGB,
                VoiceMinutes = subscription.Plan.VoiceMinutes,
                SmsCount = subscription.Plan.SmsCount
            },
            Status = subscription.Status,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            BillingCycle = subscription.BillingCycle,
            AddOns = subscription.SubscriptionAddOns.Select(sa => new AddOnDto
            {
                Id = sa.AddOn.Id,
                Name = sa.AddOn.Name,
                Description = sa.AddOn.Description,
                Price = sa.AddOn.Price,
                Type = sa.AddOn.Type,
                Value = sa.AddOn.Value
            }).ToList(),
            CreatedAt = subscription.CreatedAt,
            UpdatedAt = subscription.UpdatedAt
        };
    }
}
