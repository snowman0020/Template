using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Template.Domain.DTO;
using Template.Helper.DataCache;
using Template.Infrastructure;

namespace Template.UnitTest
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
        public void DataCacheFlowTest()
        {
            string dataId = "7177ea0f-af20-499d-a5fc-c0b0d96501a1";
            string refreshToken = "e84c2159-737a-46cd-8107-b7b1f5b5d760";

            var dataToCache = new LoginDTO();
            dataToCache.RefreshToken = refreshToken;

            DateTime expiredDate = DateTime.Now;

            _dataCache.SetDataToCache(dataToCache, dataId, expiredDate);

            var dataFromCache = _dataCache.GetDataFromCache(dataId);

            Assert.NotNull(dataFromCache);
            Assert.NotNull(dataFromCache.RefreshToken);
            Assert.NotEmpty(dataFromCache.RefreshToken);

            Assert.Equal(refreshToken, dataFromCache.RefreshToken);

            _dataCache.RemoveKeyFromCache(dataId);

            var dataFromDeleteCache = _dataCache.GetDataFromCache(dataId);

            Assert.Null(dataFromDeleteCache.RefreshToken);
            Assert.NotEqual(refreshToken, dataFromDeleteCache.RefreshToken);
        }
    }
}