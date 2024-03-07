using APIx.Data;
using APIx.Models;
using Microsoft.EntityFrameworkCore;

namespace APIx.Repositories;

public class KeysRepository(AppDBContext appDBContext)
{
    private readonly AppDBContext _appDBContext = appDBContext;
    public async Task<PixKey[]> RetrieveKeysByUserId(int userId)
    {
        return await _appDBContext.PixKey
        .Include(p => p.PaymentProviderAccount)
        .Where(p => p.PaymentProviderAccount.UserId == userId)
        .ToArrayAsync();
    }
    public async Task<PixKey> CreateKey(PixKey pixKey)
    {
        var entry = _appDBContext.PixKey.Add(pixKey);
        await _appDBContext.SaveChangesAsync();
        
        return entry.Entity;
    }
}