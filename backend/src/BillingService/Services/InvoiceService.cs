using BillingService.Data;
using BillingService.Models;
using Shared.Messaging;
using Shared.Events;

namespace BillingService.Services;

public class InvoiceService : IInvoiceService
{
    private readonly BillingDbContext _context;
    private readonly IEventPublisher _eventPublisher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(
        BillingDbContext context,
        IEventPublisher eventPublisher,
        IConfiguration configuration,
        ILogger<InvoiceService> logger)
    {
        _context = context;
        _eventPublisher = eventPublisher;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<InvoiceDto> GenerateInvoiceAsync(Guid userId, List<InvoiceItemDto> items)
    {
        var totalAmount = items.Sum(i => i.Amount * i.Quantity);
        var invoiceNumber = $"INV-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            InvoiceNumber = invoiceNumber,
            InvoiceDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14),
            Status = "Pending",
            TotalAmount = totalAmount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var itemDto in items)
        {
            invoice.Items.Add(new InvoiceItem
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoice.Id,
                Description = itemDto.Description,
                Amount = itemDto.Amount,
                Quantity = itemDto.Quantity
            });
        }

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        await _eventPublisher.PublishAsync(
            _configuration["AWS:SnsTopicArn"]!,
            new InvoiceGeneratedEvent
            {
                UserId = userId,
                InvoiceId = invoice.Id,
                InvoiceNumber = invoiceNumber,
                Amount = totalAmount,
                DueDate = invoice.DueDate,
                Timestamp = DateTime.UtcNow
            });

        _logger.LogInformation("Generated invoice {InvoiceNumber} for user {UserId}", invoiceNumber, userId);

        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            Status = invoice.Status,
            TotalAmount = invoice.TotalAmount,
            Items = items
        };
    }
}
