using System;
using Microsoft.Extensions.Caching.Distributed;
using BusinessLayer.Interface;

namespace BusinessLayer.Service
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public void SetCache(string key, string value, int expirationInMinutes)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationInMinutes)
            };

            _cache.SetString(key, value, options);
            Console.WriteLine($"Cache Set: {key} - {value}");
        }

        public string GetCache(string key)
        {
            var value = _cache.GetString(key);
            Console.WriteLine($"Cache Get: {key} - {value}");
            return value ?? string.Empty;
        }

        public void RemoveCache(string key)
        {
            _cache.Remove(key);
            Console.WriteLine($"Cache Removed: {key}");
        }
    }
}
