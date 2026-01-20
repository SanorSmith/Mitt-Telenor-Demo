namespace Shared.Events;

public class InvoiceGeneratedEvent
{
    public Guid InvoiceId { get; set; }
    public Guid UserId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Total { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime GeneratedAt { get; set; }
    public DateTime Timestamp { get; set; }
}
