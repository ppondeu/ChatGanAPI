using System;
using System.Text.Json;
using ChatApi.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace ChatApi.Services;

public class RedisService(IDistributedCache cache): IRedisService
{
    private readonly IDistributedCache _cache = cache;

    public async Task SetCacheAsync<T>(string key, T value, DistributedCacheEntryOptions options)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, serializedValue, options);
    }

    public async Task<T?> GetCacheAsync<T>(string key)
    {
        var serializedValue = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(serializedValue)) return default;
        return JsonSerializer.Deserialize<T>(serializedValue);
    }

    public async Task RemoveCacheAsync(string key) {
        await _cache.RemoveAsync(key);
    }
}
