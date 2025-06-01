using Template.Domain.DTO;
using Template.Domain.Filter;
using Template.Domain.Paging;
using Template.Domain.SortBy;

namespace Template.Service.IServices
{
    public interface IUserService
    {
        Task<PageList<UserDTO>> GetUserListAsync(UserFilter? filter, PageParam pageParam, UserSortBy? sortBy);
        Task<UserDTO> GetUserByIdAsync(string Id);
        Task<UserDTO> AddUserAsync(UserRequest input, string userAddId);
        Task<UserDTO> UpdateUserAsync(string Id, UserRequest input, string userUpdateId);
        Task DeleteUserAsync(string Id, string userDeleteId);
    }
}