using Template.Domain.DTO;

namespace Template.Helper.DataCache
{
    public interface IDataCache
    {
        void SetDataToCache(LoginDTO input, string Id, DateTime expiredDate);
        LoginCacheDTO GetDataFromCache(string Id);
        void RemoveKeyFromCache(string Id);
    }
}