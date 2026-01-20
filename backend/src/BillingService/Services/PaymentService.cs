using Microsoft.EntityFrameworkCore;
using BillingService.Data;
using BillingService.Models;

namespace BillingService.Services;

public class PaymentService : IPaymentService
{
    private readonly BillingDbContext _context;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(BillingDbContext context, ILogger<PaymentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaymentDto> MakePaymentAsync(Guid userId, MakePaymentRequest request)
    {
        var invoice = await _context.Invoices.FindAsync(request.InvoiceId);
        if (invoice == null || invoice.UserId != userId)
            throw new KeyNotFoundException("Invoice not found");

        if (invoice.Status == "Paid")
            throw new InvalidOperationException("Invoice already paid");

        var paymentMethod = await _context.PaymentMethods.FindAsync(request.PaymentMethodId);
        if (paymentMethod == null || paymentMethod.UserId != userId)
            throw new KeyNotFoundException("Payment method not found");

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            InvoiceId = request.InvoiceId,
            UserId = userId,
            Amount = request.Amount,
            Status = "Completed",
            PaymentMethod = paymentMethod.Type,
            TransactionId = $"TXN-{Guid.NewGuid().ToString()[..12].ToUpper()}",
            PaymentDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);

        var totalPaid = await _context.Payments
            .Where(p => p.InvoiceId == request.InvoiceId && p.Status == "Completed")
            .SumAsync(p => p.Amount) + request.Amount;

        if (totalPaid >= invoice.TotalAmount)
        {
            invoice.Status = "Paid";
            invoice.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Payment of {Amount} made for invoice {InvoiceId} by user {UserId}",
            request.Amount, request.InvoiceId, userId);

        return new PaymentDto
        {
            Id = payment.Id,
            Amount = payment.Amount,
            Status = payment.Status,
            PaymentMethod = payment.PaymentMethod,
            TransactionId = payment.TransactionId,
            PaymentDate = payment.PaymentDate
        };
    }

    public async Task<List<PaymentMethodDto>> GetPaymentMethodsAsync(Guid userId)
    {
        var methods = await _context.PaymentMethods
            .Where(pm => pm.UserId == userId)
            .OrderByDescending(pm => pm.IsDefault)
            .ThenByDescending(pm => pm.CreatedAt)
            .ToListAsync();

        return methods.Select(pm => new PaymentMethodDto
        {
            Id = pm.Id,
            Type = pm.Type,
            LastFourDigits = pm.LastFourDigits,
            ExpiryDate = pm.ExpiryDate,
            IsDefault = pm.IsDefault
        }).ToList();
    }

    public async Task<PaymentMethodDto> AddPaymentMethodAsync(Guid userId, AddPaymentMethodRequest request)
    {
        if (request.IsDefault)
        {
            var existingMethods = await _context.PaymentMethods
                .Where(pm => pm.UserId == userId)
                .ToListAsync();

            foreach (var method in existingMethods)
            {
                method.IsDefault = false;
            }
        }

        var paymentMethod = new PaymentMethod
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = request.Type,
            LastFourDigits = request.LastFourDigits,
            ExpiryDate = request.ExpiryDate,
            IsDefault = request.IsDefault,
            CreatedAt = DateTime.UtcNow
        };

        _context.PaymentMethods.Add(paymentMethod);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Added payment method for user {UserId}", userId);

        return new PaymentMethodDto
        {
            Id = paymentMethod.Id,
            Type = paymentMethod.Type,
            LastFourDigits = paymentMethod.LastFourDigits,
            ExpiryDate = paymentMethod.ExpiryDate,
            IsDefault = paymentMethod.IsDefault
        };
    }

    public async Task<bool> DeletePaymentMethodAsync(Guid userId, Guid paymentMethodId)
    {
        var paymentMethod = await _context.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.Id == paymentMethodId && pm.UserId == userId);

        if (paymentMethod == null) return false;

        _context.PaymentMethods.Remove(paymentMethod);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted payment method {PaymentMethodId} for user {UserId}",
            paymentMethodId, userId);

        return true;
    }
}
