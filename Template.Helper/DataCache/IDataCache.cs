using Template.Domain.DTO;

namespace Template.Helper.DataCache
{
    public interface IDataCache
    {
        Task SetDataToCacheAsync(LoginDTO input, string Id, DateTime expiredDate);
        Task<LoginCacheDTO> GetDataFromCacheAsync(string Id);
        Task RemoveKeyFromCacheAsync(string Id);
    }
}