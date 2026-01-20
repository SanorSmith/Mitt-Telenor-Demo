namespace Shared.Events;

public class SubscriptionChangedEvent
{
    public Guid SubscriptionId { get; set; }
    public Guid UserId { get; set; }
    public Guid OldPlanId { get; set; }
    public Guid NewPlanId { get; set; }
    public Guid PlanId { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public DateTime Timestamp { get; set; }
}
