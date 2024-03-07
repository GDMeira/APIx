using APIx.Data;
using APIx.Models;
using Microsoft.EntityFrameworkCore;

namespace APIx.Repositories;

public class AccountsRepository(AppDBContext appDBContext)
{
    private readonly AppDBContext _appDBContext = appDBContext;
    public async Task<PaymentProviderAccount?> RetrieveAccount(string number, string agency)
    {
        return await _appDBContext.PaymentProviderAccount
                        .FirstOrDefaultAsync(a => a.Number == number && a.Agency == agency);
    }

    public async Task<PaymentProviderAccount> CreateAccount(PaymentProviderAccount paymentProviderAccount)
    {
        var entry = _appDBContext.PaymentProviderAccount.Add(paymentProviderAccount);
        await _appDBContext.SaveChangesAsync();

        return entry.Entity;
    }
}