using APIx.Data;
using APIx.Models;
using Microsoft.EntityFrameworkCore;

namespace APIx.Repositories;

public class PaymentsRepository(AppDBContext appDBContext, CacheRepository cacheRepository)
{
    private readonly AppDBContext _appDBContext = appDBContext;
    private readonly CacheRepository _cacheRepository = cacheRepository;
    public async Task<Payment> CreatePayment(Payment payment)
    {
        await _appDBContext.Payment.AddAsync(payment);
        await _appDBContext.SaveChangesAsync();

        await _cacheRepository
            .SetCachedData($"paymentIdempotence-{payment.PixKeyId}:{payment.PaymentProviderAccountId}:{payment.Amount}", payment, TimeSpan.FromSeconds(30));
        await _cacheRepository
            .SetCachedData($"payment-{payment.Id}", payment, TimeSpan.FromMinutes(2));

        return payment;
    }

    public async Task<Payment?> RetrievePaymentByValueAndPixKeyAndAccount(Payment payment)
    {
        Payment? paymentCached = await _cacheRepository.GetCachedData<Payment>($"paymentIdempotence-{payment.PixKeyId}:{payment.PaymentProviderAccountId}:{payment.Amount}");

        if (paymentCached != null)
        {
            return paymentCached;
        }

        Payment? paymentDB = await _appDBContext.Payment
            .FirstOrDefaultAsync(p => p.PixKeyId == payment.PixKeyId 
                && p.PaymentProviderAccountId == payment.PaymentProviderAccountId
                && p.Amount == payment.Amount
                && p.CreatedAt > DateTime.UtcNow.AddSeconds(-30)
            );

        return paymentDB;
    }
}