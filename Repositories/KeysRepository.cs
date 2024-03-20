using APIx.Data;
using APIx.Models;
using Microsoft.EntityFrameworkCore;

namespace APIx.Repositories;

public class KeysRepository(AppDBContext appDBContext, CacheRepository cache)
{
    private readonly AppDBContext _appDBContext = appDBContext;
    private readonly CacheRepository _cache = cache;
    public async Task<PixKey[]> RetrieveKeysByUserId(int userId)
    {
        return await _appDBContext.PixKey
        .Include(p => p.PaymentProviderAccount)
        .Where(p => p.PaymentProviderAccount.UserId == userId)
        .ToArrayAsync();
    }
    public async Task<PixKey> CreateKey(PixKey pixKey)
    {
        var entry = await _appDBContext.PixKey.AddAsync(pixKey);
        await _appDBContext.SaveChangesAsync();
        
        return entry.Entity;
    }

    public async Task<PixKey> CreateKey(PixKey pixKey, PaymentProviderAccount account)
    {
        account.PixKeys = [pixKey];
        var entry = await _appDBContext.PaymentProviderAccount
            .AddAsync(account);
        await _appDBContext.SaveChangesAsync();
        
        return entry.Entity.PixKeys.Last();
    }
    

    public async Task<PixKey?> RetrieveKeyByValue(string value)
    {
        PixKey? cachedKey = await _cache.GetCachedData<PixKey>($"pixKey-{value}");
        
        if (cachedKey != null)
        {
            return cachedKey;
        }

        PixKey? keyDB = await _appDBContext.PixKey
            .AsSplitQuery()
            .Include(p => p.PaymentProviderAccount.User)
            .Include(p => p.PaymentProviderAccount.PaymentProvider)
            .FirstOrDefaultAsync(p => p.Value == value);

        if (keyDB != null)
        {
            await _cache.SetCachedData($"pixKey-{value}", keyDB);
        }

        return keyDB;
    }
}