using Template.Domain.DTO;

namespace Template.Domain.Paging
{
    public class UserPaging
    {
        public List<UserDTO>? UserList { get; set; }
        public PageOutput? PageOutput { get; set; }
    }
}
