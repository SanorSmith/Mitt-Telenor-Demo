using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using SubscriptionService.Data;
using SubscriptionService.Models;

namespace SubscriptionService.Services;

public class PlanService : IPlanService
{
    private readonly SubscriptionDbContext _context;
    private readonly IDistributedCache _cache;

    public PlanService(SubscriptionDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<PlanDto>> GetAllPlansAsync()
    {
        const string cacheKey = "plans:all";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<List<PlanDto>>(cached) ?? new List<PlanDto>();
        }

        var plans = await _context.Plans
            .Where(p => p.IsActive)
            .Select(p => new PlanDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DataAllowanceGB = p.DataAllowanceGB,
                VoiceMinutes = p.VoiceMinutes,
                SmsCount = p.SmsCount
            })
            .ToListAsync();

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(plans),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });

        return plans;
    }

    public async Task<List<AddOnDto>> GetAllAddOnsAsync()
    {
        const string cacheKey = "addons:all";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<List<AddOnDto>>(cached) ?? new List<AddOnDto>();
        }

        var addOns = await _context.AddOns
            .Where(a => a.IsActive)
            .Select(a => new AddOnDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Price = a.Price,
                Type = a.Type,
                Value = a.Value
            })
            .ToListAsync();

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(addOns),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });

        return addOns;
    }
}
