using Template.Domain.DTO;

namespace Template.Helper.DataCache
{
    public interface IDataCache
    {
        Task SetDataToCache(LoginDTO input, string Id, DateTime expiredDate);
        Task<LoginCacheDTO> GetDataFromCache(string Id);
        Task RemoveKeyFromCache(string Id);
    }
}