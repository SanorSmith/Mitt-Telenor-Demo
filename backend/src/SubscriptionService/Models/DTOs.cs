using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Models;

public class PlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DataAllowanceGB { get; set; }
    public int VoiceMinutes { get; set; }
    public int SmsCount { get; set; }
}

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public PlanDto Plan { get; set; } = null!;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string BillingCycle { get; set; } = string.Empty;
    public List<AddOnDto> AddOns { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class AddOnDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Value { get; set; }
}

public class CreateSubscriptionRequest
{
    [Required]
    public Guid PlanId { get; set; }

    [Required]
    [RegularExpression("^(Monthly|Yearly)$")]
    public string BillingCycle { get; set; } = "Monthly";

    public List<Guid> AddOnIds { get; set; } = new();
}

public class UpdateSubscriptionRequest
{
    [Required]
    public Guid PlanId { get; set; }
}
