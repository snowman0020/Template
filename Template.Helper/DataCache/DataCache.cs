using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using Template.Domain.AppSetting;

namespace Template.Helper.DataCache
{
    public class DataCache : IDataCache
    {
        private readonly ILogger<DataCache> _logger;
        private readonly IDistributedCache _cache;
        private readonly RedisData _redisData;

        public DataCache(ILogger<DataCache> logger, IDistributedCache cache, IOptions<RedisData> redisData)
        {
            _logger = logger;
            _cache = cache;
            _redisData = redisData.Value;
        }
        public T? GetData<T>(string key)
        {
            _logger.LogInformation($"call: GetData");

            var data = _cache.GetString(key);

            if (data is null)
            {
                return default(T);
            }

            _logger.LogDebug($"data: {data}");

            return JsonConvert.DeserializeObject<T>(data);
        }

        public void SetData<T>(string key, T data)
        {
            _logger.LogInformation($"call: SetData");

            int absoluteExpirationRelativeToNow = _redisData.AbsoluteExpirationRelativeToNow;
            int slidingExpiration = _redisData.SlidingExpiration;

            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(absoluteExpirationRelativeToNow),
                SlidingExpiration = TimeSpan.FromMinutes(slidingExpiration),
            };

            _logger.LogDebug($"data: {JsonConvert.SerializeObject(data)}");

            _cache.SetString(key, JsonConvert.SerializeObject(data), options);
        }
    }
}