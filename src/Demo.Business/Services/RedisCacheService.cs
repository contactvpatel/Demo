using System;
using System.Threading.Tasks;
using Demo.Business.Interfaces;

namespace Demo.Business.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly Core.Services.IRedisCacheService _redisCacheService;

        public RedisCacheService(Core.Services.IRedisCacheService redisCacheService)
        {
            _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
        }

        public async Task SetCacheData<T>(string cacheKey, T data, TimeSpan? absoluteExpireTime = null,
            TimeSpan? unusedExpireTime = null)
        {
            await _redisCacheService.SetCacheData(cacheKey, data, absoluteExpireTime, unusedExpireTime);
        }

        public async Task<T> GetCachedData<T>(string cacheKey)
        {
            return await _redisCacheService.GetCachedData<T>(cacheKey);
        }
    }
}