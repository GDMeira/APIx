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
            .FirstOrDefaultAsync(
                a => a.PaymentProviderId == account.PaymentProviderId 
                && a.UserId == account.UserId
                && a.Number == account.Number
                && a.Agency == account.Agency);
    }

    public async Task<PaymentProviderAccount> CreateAccount(PaymentProviderAccount paymentProviderAccount)
    {
        var entry = _appDBContext.PaymentProviderAccount.Add(paymentProviderAccount);
        await _appDBContext.SaveChangesAsync();

        return entry.Entity;
    }
}