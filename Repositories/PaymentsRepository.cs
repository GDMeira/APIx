using APIx.Data;
using APIx.Models;
using Microsoft.EntityFrameworkCore;

namespace APIx.Repositories;

public class PaymentsRepository(AppDBContext appDBContext)
{
    private readonly AppDBContext _appDBContext = appDBContext;
    public async Task<Payment> CreatePayment(Payment payment)
    {
        var entry = await _appDBContext.Payment.AddAsync(payment);
        await _appDBContext.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task<Payment?> RetrievePaymentByValueAndPixKeyAndAccount(Payment payment)
    {
        return await _appDBContext.Payment
            .FirstOrDefaultAsync(p => p.PixKeyId == payment.PixKeyId 
                && p.PaymentProviderAccountId == payment.PaymentProviderAccountId
                && p.Amount == payment.Amount
                && p.CreatedAt > DateTime.UtcNow.AddSeconds(-30)
            );
    }
}