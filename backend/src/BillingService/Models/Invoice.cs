namespace BillingService.Models;

public class Invoice
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? PdfUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class InvoiceItem
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Quantity { get; set; }

    public Invoice Invoice { get; set; } = null!;
}

public class Payment
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public Invoice Invoice { get; set; } = null!;
}

public class PaymentMethod
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? LastFourDigits { get; set; }
    public string? ExpiryDate { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
}
