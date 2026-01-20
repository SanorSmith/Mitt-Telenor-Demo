using Microsoft.EntityFrameworkCore;
using BillingService.Data;
using BillingService.Models;

namespace BillingService.Services;

public class BillingService : IBillingService
{
    private readonly BillingDbContext _context;
    private readonly ILogger<BillingService> _logger;

    public BillingService(BillingDbContext context, ILogger<BillingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<InvoiceDto>> GetUserInvoicesAsync(Guid userId)
    {
        var invoices = await _context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Payments)
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();

        return invoices.Select(MapToDto).ToList();
    }

    public async Task<InvoiceDto?> GetInvoiceByIdAsync(Guid userId, Guid invoiceId)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == invoiceId && i.UserId == userId);

        return invoice == null ? null : MapToDto(invoice);
    }

    public async Task<List<PaymentDto>> GetUserPaymentsAsync(Guid userId)
    {
        var payments = await _context.Payments
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

        return payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            Amount = p.Amount,
            Status = p.Status,
            PaymentMethod = p.PaymentMethod,
            TransactionId = p.TransactionId,
            PaymentDate = p.PaymentDate
        }).ToList();
    }

    private static InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            Status = invoice.Status,
            TotalAmount = invoice.TotalAmount,
            PdfUrl = invoice.PdfUrl,
            Items = invoice.Items.Select(item => new InvoiceItemDto
            {
                Description = item.Description,
                Amount = item.Amount,
                Quantity = item.Quantity
            }).ToList(),
            Payments = invoice.Payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                Amount = p.Amount,
                Status = p.Status,
                PaymentMethod = p.PaymentMethod,
                TransactionId = p.TransactionId,
                PaymentDate = p.PaymentDate
            }).ToList()
        };
    }
}
