using System.Text;
using GitHubApp.Api.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace GitHubApp.Api.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<T?> Get<T>(string key)
    {
        if (!_cache.TryGetValue(key, out var response))
            return Task.FromResult<T?>(default);

        return Task.FromResult((T?)response);
    }

    public Task<bool> Set<T>(string key, T value, TimeSpan ttl)
    {
        var response = _cache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.UtcNow.Add(ttl),
            Priority = CacheItemPriority.High,
            Size = Encoding.UTF8.GetByteCount(key)
        });

        return Task.FromResult(response is not null);
    }
}