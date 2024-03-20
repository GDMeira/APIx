using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace APIx.Repositories;

public class CacheRepository(IDistributedCache cache)
{
    private readonly IDistributedCache _cache = cache;

    public async Task<T?> GetCachedData<T>(string key)
        where T : class
    {
        string? data = await _cache.GetStringAsync(key);

        if (data is null)
        {
            return null;
        }

        return JsonConvert.DeserializeObject<T>(data);
    }

    public async Task SetCachedData<T>(string key, T data, TimeSpan? expiration = null)
        where T : class
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        };

        var serializedData = JsonConvert.SerializeObject(data);

        await _cache.SetStringAsync(key, serializedData, options);
    }

    public async Task RemoveCachedData(string key)
    {
        await _cache.RemoveAsync(key);
    }
}