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
        .AsNoTracking()
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
    

    public async Task<PixKey?> RetrieveKeyByValue(string value, bool getSetCache = true)
    {
        if (getSetCache)
        {
            PixKey? keyCache = await _cache.GetCachedData<PixKey>($"pixKey-{value}");
            if (keyCache != null) return keyCache;
        }

        PixKey? keyDB = await _appDBContext.PixKey
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.PaymentProviderAccount.User)
            .Include(p => p.PaymentProviderAccount.PaymentProvider)
            .FirstOrDefaultAsync(p => p.Value == value);

        if (keyDB != null && getSetCache)
        {
            await _cache.SetCachedData($"pixKey-{value}", keyDB);
        }

        return keyDB;
    }

    public async Task<int> CountKeysByUserId(int userId)
    {
        return await _appDBContext.PixKey
            .AsNoTracking()
            .CountAsync(p => p.PaymentProviderAccount.UserId == userId);
    }

    public async Task<int> CountKeysByUserIdAndProviderId(int userId, int providerId)
    {
        return await _appDBContext.PixKey
            .AsNoTracking()
            .CountAsync(p => p.PaymentProviderAccount.UserId == userId &&
                p.PaymentProviderAccount.PaymentProviderId == providerId);
    }

}