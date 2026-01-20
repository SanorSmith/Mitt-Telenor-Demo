namespace Shared.Events;

public class UsageAlertEvent
{
    public Guid UserId { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public decimal ThresholdPercentage { get; set; }
    public decimal CurrentUsage { get; set; }
    public decimal TotalAllowance { get; set; }
    public decimal Allowance { get; set; }
    public decimal PercentageUsed { get; set; }
    public DateTime AlertedAt { get; set; }
    public DateTime Timestamp { get; set; }
}
