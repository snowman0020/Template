using Template.Domain.DTO;
using Template.Domain.Filter;
using Template.Domain.Paging;
using Template.Domain.SortBy;

namespace Template.Service.IServices
{
    public interface IUserService
    {
        Task<UserPaging> GetUserListAsync(UserFilter userFilter, PageParam pageParam, UserSortBy? userSortBy);
        Task<UserDTO> GetUserByIdAsync(string ID);
        Task<UserDTO> CreateUserAsync(UserDTO input);
        Task<UserDTO> UpdateUserAsync(string ID, UserDTO input);
        Task<bool> DeleteUserAsync(string ID);
    }
}
