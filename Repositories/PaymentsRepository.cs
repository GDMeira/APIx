using System.Data;
using APIx.Data;
using APIx.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace APIx.Repositories;

public class PaymentsRepository(AppDBContext appDBContext, CacheRepository cacheRepository)
{
    private readonly AppDBContext _appDBContext = appDBContext;
    private readonly CacheRepository _cacheRepository = cacheRepository;

    public async Task<Payment> CreatePayment(Payment payment, PaymentProviderAccount account, PixKey pixKey)
    {
        await _appDBContext.Payment
            .AddAsync(payment);
        await _appDBContext.SaveChangesAsync();

        if (account.PaymentProvider == null)
        {
            account.PaymentProvider = await _appDBContext.PaymentProvider
                .AsNoTracking()
                .FirstAsync(p => p.Id == account.PaymentProviderId);
        }

        payment.PixKey = pixKey;
        payment.PaymentProviderAccount = account;
        _ = _cacheRepository
            .SetCachedData($"paymentIdempotence-{payment.PixKeyId}:{payment.PaymentProviderAccountId}:{payment.Amount}", new { Id = payment.Id }, TimeSpan.FromSeconds(30));
        _ = _cacheRepository
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

    public IDbTransaction BeginTransaction()
    {
        return _appDBContext.Database.BeginTransaction().GetDbTransaction();
    }
}