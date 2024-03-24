using APIx.Data;
using APIx.Models;
using Microsoft.EntityFrameworkCore;

namespace APIx.Repositories;

public class AccountsRepository(AppDBContext appDBContext)
{
    private readonly AppDBContext _appDBContext = appDBContext;
    public async Task<PaymentProviderAccount?> RetrieveAccount(PaymentProviderAccount account)
    {
        return await _appDBContext.PaymentProviderAccount
            .AsNoTracking()
            .Include(a => a.PaymentProvider)
            .FirstOrDefaultAsync(
                a => a.PaymentProviderId == account.PaymentProviderId 
                && a.UserId == account.UserId
                && a.Number == account.Number
                && a.Agency == account.Agency);
    }

    public async Task<PaymentProviderAccount> CreateAccount(PaymentProviderAccount paymentProviderAccount)
    {
        await _appDBContext.PaymentProviderAccount.AddAsync(paymentProviderAccount);
        await _appDBContext.SaveChangesAsync();

        return paymentProviderAccount;
    }
}