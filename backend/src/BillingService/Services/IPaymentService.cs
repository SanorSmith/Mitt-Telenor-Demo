using BillingService.Models;

namespace BillingService.Services;

public interface IPaymentService
{
    Task<PaymentDto> MakePaymentAsync(Guid userId, MakePaymentRequest request);
    Task<List<PaymentMethodDto>> GetPaymentMethodsAsync(Guid userId);
    Task<PaymentMethodDto> AddPaymentMethodAsync(Guid userId, AddPaymentMethodRequest request);
    Task<bool> DeletePaymentMethodAsync(Guid userId, Guid paymentMethodId);
}
