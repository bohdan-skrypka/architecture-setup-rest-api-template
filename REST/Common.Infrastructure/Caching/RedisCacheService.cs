using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly CacheConfiguration _cacheConfig;
        private DistributedCacheEntryOptions _distributedCacheOptions;

        public RedisCacheService(IDistributedCache memoryCache, IOptions<CacheConfiguration> cacheConfig)
        {
            _distributedCache = memoryCache;
            _cacheConfig = cacheConfig.Value;
            if (_cacheConfig != null)
            {
                _distributedCacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.UtcNow.AddHours(_cacheConfig.AbsoluteExpirationInHours),
                    SlidingExpiration = TimeSpan.FromMinutes(_cacheConfig.SlidingExpirationInMinutes),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheConfig.SlidingExpirationInMinutes),
                };
            }
        }

        public void Remove(string cacheKey)
        {
            _distributedCache.Remove(cacheKey);
        }

        public T Set<T>(string cacheKey, T value)
        {
//            _distributedCache.Set(cacheKey, value, _distributedCacheOptions);
            return default;
        }

        public bool TryGet<T>(string cacheKey, out T value)
        {
            // _distributedCache.getTryGetValue(cacheKey, out value);
           value = default;
            return false;
            //return value == null
            //    ? false
            //    : true;
        }
    }
}
