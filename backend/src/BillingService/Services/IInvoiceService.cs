using BillingService.Models;

namespace BillingService.Services;

public interface IInvoiceService
{
    Task<InvoiceDto> GenerateInvoiceAsync(Guid userId, List<InvoiceItemDto> items);
}
