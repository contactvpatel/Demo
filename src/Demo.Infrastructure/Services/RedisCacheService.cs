using Demo.Core.Models;
using Demo.Core.Services;
using Demo.Util.Logging;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo.Infrastructure.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisCacheModel _redisCacheModel;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IDistributedCache distributedCache, RedisCacheModel redisCacheModel,
            ILogger<RedisCacheService> logger)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _redisCacheModel = redisCacheModel ?? throw new ArgumentNullException(nameof(redisCacheModel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SetCacheData<T>(string cacheKey, T data, TimeSpan? absoluteExpireTime = null,
            TimeSpan? unusedExpireTime = null)
        {
            if (_redisCacheModel.Enabled)
            {
                if (data == null)
                    return;

                try
                {
                    var options = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = absoluteExpireTime ??
                                                          TimeSpan.FromSeconds(
                                                              _redisCacheModel.DefaultCacheTimeInSeconds),
                        SlidingExpiration = unusedExpireTime
                    };

                    var jsonData = JsonConvert.SerializeObject(data);
                    await _distributedCache.SetStringAsync(cacheKey, jsonData, options);
                }
                catch (Exception ex)
                {
                    _logger.LogErrorExtension(ex.Message, ex);
                }
            }
        }

        public async Task<T> GetCachedData<T>(string cacheKey)
        {
            if (!_redisCacheModel.Enabled) return default;
            try
            {
                var jsonData = await _distributedCache.GetStringAsync(cacheKey);
                return jsonData is null ? default : JsonConvert.DeserializeObject<T>(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogErrorExtension(ex.Message, ex);
                return default;
            }
        }
    }
}
