using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BillingService.Models;
using BillingService.Services;

namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BillingController : ControllerBase
{
    private readonly IBillingService _billingService;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<BillingController> _logger;

    public BillingController(
        IBillingService billingService,
        IPaymentService paymentService,
        ILogger<BillingController> logger)
    {
        _billingService = billingService;
        _paymentService = paymentService;
        _logger = logger;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    [HttpGet("invoices")]
    public async Task<ActionResult<List<InvoiceDto>>> GetInvoices()
    {
        var userId = GetUserId();
        var invoices = await _billingService.GetUserInvoicesAsync(userId);
        return Ok(invoices);
    }

    [HttpGet("invoices/{invoiceId}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(Guid invoiceId)
    {
        var userId = GetUserId();
        var invoice = await _billingService.GetInvoiceByIdAsync(userId, invoiceId);

        if (invoice == null)
            return NotFound(new { message = "Invoice not found" });

        return Ok(invoice);
    }

    [HttpGet("payments")]
    public async Task<ActionResult<List<PaymentDto>>> GetPayments()
    {
        var userId = GetUserId();
        var payments = await _billingService.GetUserPaymentsAsync(userId);
        return Ok(payments);
    }

    [HttpPost("payments")]
    public async Task<ActionResult<PaymentDto>> MakePayment([FromBody] MakePaymentRequest request)
    {
        var userId = GetUserId();

        try
        {
            var payment = await _paymentService.MakePaymentAsync(userId, request);
            return Ok(payment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("payment-methods")]
    public async Task<ActionResult<List<PaymentMethodDto>>> GetPaymentMethods()
    {
        var userId = GetUserId();
        var methods = await _paymentService.GetPaymentMethodsAsync(userId);
        return Ok(methods);
    }

    [HttpPost("payment-methods")]
    public async Task<ActionResult<PaymentMethodDto>> AddPaymentMethod([FromBody] AddPaymentMethodRequest request)
    {
        var userId = GetUserId();
        var method = await _paymentService.AddPaymentMethodAsync(userId, request);
        return CreatedAtAction(nameof(GetPaymentMethods), new { }, method);
    }

    [HttpDelete("payment-methods/{paymentMethodId}")]
    public async Task<ActionResult> DeletePaymentMethod(Guid paymentMethodId)
    {
        var userId = GetUserId();
        var success = await _paymentService.DeletePaymentMethodAsync(userId, paymentMethodId);

        if (!success)
            return NotFound(new { message = "Payment method not found" });

        return NoContent();
    }
}
