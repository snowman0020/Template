namespace Template.Domain.SortBy
{
    public class UserSortBy
    {
        public UserEnumSortBy? UserEnumSortBy { get; set; }
        public bool Ascending { get; set; }
    }

    public enum UserEnumSortBy
    {
        FullName,
        Phone,
        Email
    }
}
