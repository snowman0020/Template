using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using Template.Domain.AppSetting;
using Template.Domain.DTO;
using Template.Infrastructure.Models;

namespace Template.Helper.DataCache
{
    public class DataCache : IDataCache
    {
        private readonly ILogger<DataCache> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly RedisData _redisData;

        public DataCache(ILogger<DataCache> logger, IDistributedCache distributedCache, IOptions<RedisData> redisData)
        {
            _logger = logger;
            _distributedCache = distributedCache;
            _redisData = redisData.Value;
        }
 
        public void SetDataToCache(LoginDTO input, string Id, DateTime expiredDate)
        {
            if (input != null)
            {
                _logger.LogInformation($"call: SetDataToCache");

                var dataTokenInCache = new LoginCacheDTO();

                dataTokenInCache.Token = input.Token;
                dataTokenInCache.RefreshToken = input.RefreshToken;
                dataTokenInCache.Email = input.Email;
                dataTokenInCache.Expires = input.Expires;
                dataTokenInCache.Email = input.Email;
                dataTokenInCache.CreatedDate = input.CreatedDate;
                dataTokenInCache.ExpiredDate = expiredDate;

                string cacheId = _redisData.CacheId ?? "";
                cacheId = cacheId.Replace("{id}", Id);

                 SetData(cacheId, dataTokenInCache);

                _logger.LogDebug($"cache id: {cacheId}, data: {JsonSerializer.Serialize(dataTokenInCache)}");
            }
        }

        public LoginCacheDTO GetDataFromCache(string Id)
        {
            var result = new LoginCacheDTO();

            if (!string.IsNullOrEmpty(Id))
            {
                _logger.LogInformation($"call: SetDataFromCache");

                _logger.LogDebug($"user id: {Id}");

                string cacheId = _redisData.CacheId ?? "";
                cacheId = cacheId.Replace("{id}", Id);

                var dataTokenInCache = _distributedCache.GetString(cacheId);
                
                if (dataTokenInCache != null)
                {
                    var dataTokenInCacheSerializer = JsonSerializer.Deserialize<LoginCacheDTO>(dataTokenInCache);

                    if (dataTokenInCacheSerializer != null)
                    {
                        result.Token = dataTokenInCacheSerializer.Token;
                        result.RefreshToken = dataTokenInCacheSerializer.RefreshToken;
                        result.Email = dataTokenInCacheSerializer.Email;
                        result.Expires = dataTokenInCacheSerializer.Expires;
                        result.Email = dataTokenInCacheSerializer.Email;
                        result.CreatedDate = dataTokenInCacheSerializer.CreatedDate;
                        result.ExpiredDate = dataTokenInCacheSerializer.ExpiredDate;
                    }

                    _logger.LogDebug($"data: {JsonSerializer.Serialize(result)}");
                }
                else
                {
                    _logger.LogDebug($"not have data");
                }
            }

            return result;
        }

        public void RemoveKeyFromCache(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                _logger.LogInformation($"call: RemoveKeyFromCache");

                string cacheId = _redisData.CacheId ?? "";
                cacheId = cacheId.Replace("{id}", Id);

                _distributedCache.Remove(cacheId);

                _logger.LogDebug($"cache id: {cacheId}, remove success");
            }
        }

        private void SetData<T>(string key, T data)
        {
            _logger.LogInformation($"call: SetData");

            int absoluteExpirationRelativeToNow = _redisData.AbsoluteExpirationRelativeToNow;
            int slidingExpiration = _redisData.SlidingExpiration;

            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(absoluteExpirationRelativeToNow),
                SlidingExpiration = TimeSpan.FromMinutes(slidingExpiration),
            };

            _logger.LogDebug($"data: {JsonSerializer.Serialize(data)}");

            _distributedCache.SetString(key, JsonSerializer.Serialize(data), options);
        }

        private T? GetData<T>(string key)
        {
            _logger.LogInformation($"call: GetData");

            var data = _distributedCache.GetString(key);

            if (data is null)
            {
                return default(T);
            }

            _logger.LogDebug($"data: {data}");

            return JsonSerializer.Deserialize<T>(data);
        }
    }
}