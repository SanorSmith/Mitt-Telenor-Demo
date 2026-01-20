namespace SubscriptionService.Models;

public class Plan
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DataAllowanceGB { get; set; }
    public int VoiceMinutes { get; set; }
    public int SmsCount { get; set; }
    public bool IsActive { get; set; }
}

public class Subscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PlanId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string BillingCycle { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Plan Plan { get; set; } = null!;
    public ICollection<SubscriptionAddOn> SubscriptionAddOns { get; set; } = new List<SubscriptionAddOn>();
}

public class AddOn
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Value { get; set; }
    public bool IsActive { get; set; }
}

public class SubscriptionAddOn
{
    public Guid Id { get; set; }
    public Guid SubscriptionId { get; set; }
    public Guid AddOnId { get; set; }
    public DateTime AddedAt { get; set; }

    public Subscription Subscription { get; set; } = null!;
    public AddOn AddOn { get; set; } = null!;
}
