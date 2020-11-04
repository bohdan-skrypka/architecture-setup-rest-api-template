using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;

namespace Common.Infrastructure.Caching
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly CacheConfiguration _cacheConfig;
        private MemoryCacheEntryOptions _cacheOptions;

        public InMemoryCacheService(IMemoryCache memoryCache, IOptions<CacheConfiguration> cacheConfig)
        {
            _memoryCache = memoryCache;
            _cacheConfig = cacheConfig.Value;
            if (_cacheConfig != null)
            {
                _cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.UtcNow.AddHours(_cacheConfig.AbsoluteExpirationInHours),
                    SlidingExpiration = TimeSpan.FromMinutes(_cacheConfig.SlidingExpirationInMinutes),
                    Priority = CacheItemPriority.High,
                };
            }
        }

        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }

        public T Set<T>(string cacheKey, T value)
        {
            return _memoryCache.Set(cacheKey, value, _cacheOptions);
        }

        public bool TryGet<T>(string cacheKey, out T value)
        {
            _memoryCache.TryGetValue(cacheKey, out value);

            return value == null
                ? false
                : true;
        }
    }
}
