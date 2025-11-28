using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.Core.Entities.SaaS;
using Project.Core.Enums;
using Project.Core.Interfaces;

namespace Project.Application.BackgroundJobs.SaaS;

public class PaymentRetryWorker
{
    private readonly IRepository<SaasTenantSubscriptionPayment, Guid> _paymentRepository;
    private readonly IPaymentManager _paymentManager;
    private readonly ILogger<PaymentRetryWorker> _logger;

    public PaymentRetryWorker(
        IRepository<SaasTenantSubscriptionPayment, Guid> paymentRepository,
        IPaymentManager paymentManager,
        ILogger<PaymentRetryWorker> logger)
    {
        _paymentRepository = paymentRepository;
        _paymentManager = paymentManager;
        _logger = logger;
    }

    public async Task Execute()
    {
        _logger.LogInformation("Checking for failed payments to retry...");
        
        try
        {
            var failedPayments = await _paymentRepository.GetQueryable()
                .Where(x => x.Status == PaymentStatus.Failed 
                    && x.RetryCount < 3
                    && x.NextRetryDate.HasValue
                    && x.NextRetryDate.Value <= DateTime.UtcNow)
                .ToListAsync();

            _logger.LogInformation($"Found {failedPayments.Count} payments to retry.");
            
            foreach (var payment in failedPayments)
            {
                _logger.LogInformation($"Retrying payment {payment.Id} (attempt {payment.RetryCount + 1})");
                // TODO: Implement actual retry logic via PaymentManager
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrying failed payments.");
            throw;
        }
    }
}
