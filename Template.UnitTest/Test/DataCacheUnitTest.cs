using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Template.Domain.DTO;
using Template.Helper.DataCache;
using Template.Infrastructure;

namespace Template.UnitTest.Test
{
    public class DataCacheUnitTest : IClassFixture<Startup>, IDisposable
    {
        private readonly IDataCache _dataCache;
        private readonly TemplateDbContext _db;

        public DataCacheUnitTest(IDataCache dataCache, TemplateDbContext db, IDistributedCache distributedCache)
        {
            _dataCache = dataCache;
            _db = db;
        }

        public void Dispose()
        {
            _db.Database.EnsureDeleted();
            _db.Database.Migrate();
        }

        [Fact]
        public async Task DataCacheFlowTest()
        {
            Dispose();

            string dataId = "7177ea0f-af20-499d-a5fc-c0b0d96501a1";
            string refreshToken = "e84c2159-737a-46cd-8107-b7b1f5b5d760";

            var dataToCache = new LoginDTO();
            dataToCache.RefreshToken = refreshToken;

            DateTime expiredDate = DateTime.Now;

            await _dataCache.SetDataToCacheAsync(dataToCache, dataId, expiredDate);

            var dataFromCache = await _dataCache.GetDataFromCacheAsync(dataId);

            Assert.NotNull(dataFromCache);
            Assert.NotNull(dataFromCache.RefreshToken);
            Assert.NotEmpty(dataFromCache.RefreshToken);

            Assert.Equal(refreshToken, dataFromCache.RefreshToken);

            await _dataCache.RemoveKeyFromCacheAsync(dataId);

            var dataFromDeleteCache = await _dataCache.GetDataFromCacheAsync(dataId);

            Assert.Null(dataFromDeleteCache.RefreshToken);
            Assert.NotEqual(refreshToken, dataFromDeleteCache.RefreshToken);
        }
    }
}