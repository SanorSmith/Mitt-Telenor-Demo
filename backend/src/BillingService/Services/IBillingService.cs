using BillingService.Models;

namespace BillingService.Services;

public interface IBillingService
{
    Task<List<InvoiceDto>> GetUserInvoicesAsync(Guid userId);
    Task<InvoiceDto?> GetInvoiceByIdAsync(Guid userId, Guid invoiceId);
    Task<List<PaymentDto>> GetUserPaymentsAsync(Guid userId);
}
