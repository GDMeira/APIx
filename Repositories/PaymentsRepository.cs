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
        await _appDBContext.Payment
            .AddAsync(payment);
        await _appDBContext.SaveChangesAsync();

        var paymentDB = await _appDBContext.Payment
            .AsSplitQuery()
            .Include(p => p.PixKey)
            .Include(p => p.PixKey.PaymentProviderAccount)
            .Include(p => p.PixKey.PaymentProviderAccount.PaymentProvider)
            .Include(p => p.PaymentProviderAccount)
            .FirstAsync(p => p.Id == payment.Id);

        await _cacheRepository
            .SetCachedData($"paymentIdempotence-{paymentDB.PixKeyId}:{paymentDB.PaymentProviderAccountId}:{paymentDB.Amount}", new { Id = payment.Id }, TimeSpan.FromSeconds(30));
        await _cacheRepository
            .SetCachedData($"payment-{paymentDB.Id}", paymentDB, TimeSpan.FromMinutes(2));

        return paymentDB;
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