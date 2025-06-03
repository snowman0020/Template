using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using Template.Domain.AppSetting;
using Template.Domain.DTO;

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
 
        public async Task SetDataToCacheAsync(LoginDTO input, string Id, DateTime expiredDate)
        {
            _logger.LogInformation($"call: SetDataToCacheAsync=> Start");

            if (input != null)
            {
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

                 await SetDataAsync(cacheId, dataTokenInCache);

                _logger.LogDebug($"cache id: {cacheId}, data: {JsonSerializer.Serialize(dataTokenInCache)}");
            }

            _logger.LogInformation($"call: SetDataToCacheAsync=> Finish");
        }

        public async Task<LoginCacheDTO> GetDataFromCacheAsync(string Id)
        {
            _logger.LogInformation($"call: GetDataFromCacheAsync=> Start");

            var result = new LoginCacheDTO();

            if (!string.IsNullOrEmpty(Id))
            {
                _logger.LogDebug($"user id: {Id}");

                string cacheId = _redisData.CacheId ?? "";
                cacheId = cacheId.Replace("{id}", Id);

                var dataTokenInCache = await _distributedCache.GetStringAsync(cacheId);
                
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

            _logger.LogInformation($"call: GetDataFromCacheAsync=> Finish");

            return result;
        }

        public async Task RemoveKeyFromCacheAsync(string Id)
        {
            _logger.LogInformation($"call: RemoveKeyFromCacheAsync=> Start");

            if (!string.IsNullOrEmpty(Id))
            {
                string cacheId = _redisData.CacheId ?? "";
                cacheId = cacheId.Replace("{id}", Id);

                await _distributedCache.RemoveAsync(cacheId);

                _logger.LogDebug($"cache id: {cacheId}, remove success");
            }

            _logger.LogInformation($"call: RemoveKeyFromCacheAsync=> Finish");
        }

        private async Task SetDataAsync<T>(string key, T data)
        {
            _logger.LogInformation($"call: SetDataAsync=> Start");

            int absoluteExpirationRelativeToNow = _redisData.AbsoluteExpirationRelativeToNow;
            int slidingExpiration = _redisData.SlidingExpiration;

            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(absoluteExpirationRelativeToNow),
                SlidingExpiration = TimeSpan.FromMinutes(slidingExpiration),
            };

            _logger.LogDebug($"data: {JsonSerializer.Serialize(data)}");

            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(data), options);

            _logger.LogInformation($"call: SetDataAsync=> Finish");
        }
    }
}