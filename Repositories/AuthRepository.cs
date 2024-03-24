using APIx.Data;
using APIx.Models;
using Microsoft.EntityFrameworkCore;

namespace APIx.Repositories;

public class AuthRepository(AppDBContext appDBContext)
{
    private readonly AppDBContext _appDBContext = appDBContext;
    public async Task<PaymentProvider?> RetrievePaymentProviderByToken(string token)
    {
        return await _appDBContext.PaymentProvider
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Token == token);
    }
}