using System;
using Microsoft.Extensions.Caching.Distributed;

namespace ChatApi.Interfaces;

public interface IRedisService
{
    Task SetCacheAsync<T>(string key, T value, DistributedCacheEntryOptions options);
    Task<T?> GetCacheAsync<T>(string key);
    Task RemoveCacheAsync(string key);
}
