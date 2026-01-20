using System.ComponentModel.DataAnnotations;

namespace BillingService.Models;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? PdfUrl { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
}

public class InvoiceItemDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Quantity { get; set; }
}

public class PaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public DateTime PaymentDate { get; set; }
}

public class PaymentMethodDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? LastFourDigits { get; set; }
    public string? ExpiryDate { get; set; }
    public bool IsDefault { get; set; }
}

public class MakePaymentRequest
{
    [Required]
    public Guid InvoiceId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public Guid PaymentMethodId { get; set; }
}

public class AddPaymentMethodRequest
{
    [Required]
    [RegularExpression("^(CreditCard|DebitCard|BankAccount)$")]
    public string Type { get; set; } = string.Empty;

    [Required]
    [StringLength(4, MinimumLength = 4)]
    public string LastFourDigits { get; set; } = string.Empty;

    [RegularExpression(@"^\d{2}/\d{4}$")]
    public string? ExpiryDate { get; set; }

    public bool IsDefault { get; set; }
}
