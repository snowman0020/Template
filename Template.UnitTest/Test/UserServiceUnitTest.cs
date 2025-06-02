using Microsoft.EntityFrameworkCore;
using Template.Domain.DTO;
using Template.Domain.Filter;
using Template.Domain.Paging;
using Template.Domain.SortBy;
using Template.Infrastructure;
using Template.Service.IServices;

namespace Template.UnitTest.Test
{
    public class UserServiceUnitTest : IClassFixture<Startup>, IDisposable
    {
        private readonly IUserService _userService;
        private readonly TemplateDbContext _db;

        public UserServiceUnitTest(IUserService userService, TemplateDbContext db)
        {
            _userService = userService;
            _db = db;
        }

        public void Dispose()
        {
            _db.Database.EnsureDeleted();
            _db.Database.Migrate();
        }

        [Fact]
        public async Task UserFlowTest()
        {
            Dispose();

            string userId = "";
            string userId02 = "";

            var userRequestAdd = new UserRequest()
            {
                FirstName = "TestFirstName1",
                LastName = "TestLastName1",
                Phone = "0999999999",
                Email = "testFirstName@email.com",
                Password = "1234"
            };

            var userAdd = await _userService.AddUserAsync(userRequestAdd, "System_Unit_Test");

            Assert.NotNull(userAdd);
            Assert.NotNull(userAdd.Id);
            Assert.NotEmpty(userAdd.Id);

            userId = userAdd.Id;

            var userRequestAdd02 = new UserRequest()
            {
                FirstName = "TestFirstName2",
                LastName = "TestLastName2",
                Phone = "0911111111",
                Email = "testFirstName2@email.com",
                Password = "1234"
            };

            var userAdd02 = await _userService.AddUserAsync(userRequestAdd02, "System_Unit_Test");

            Assert.NotNull(userAdd02);
            Assert.NotNull(userAdd02.Id);
            Assert.NotEmpty(userAdd02.Id);

            userId02 = userAdd02.Id;

            var userFilter = new UserFilter()
            {
                Email = "testFirstName2@email.com"
            };

            var userPageParam = new PageParam()
            {
                Page = 1,
                PageSize = 10
            };

            var userSortBy = new UserSortBy()
            {
                Ascending = true
            };

            var userList = await _userService.GetUserListAsync(null, userPageParam, userSortBy);

            Assert.NotNull(userList);
            Assert.True(userList.Count > 0);

            var userFilterList = await _userService.GetUserListAsync(userFilter, userPageParam, userSortBy);

            Assert.NotNull(userFilterList);
            Assert.True(userList.Count > 0);

            var userRequestUpdate = new UserRequest()
            {
                FirstName = "TestFirstName12",
                LastName = "TestLastName12",
                Phone = "0999999999",
                Email = "testFirstName@email.com",
            };

            var userUpdate = await _userService.UpdateUserAsync(userId, userRequestUpdate, "System_Unit_Test");

            Assert.NotNull(userUpdate);
            Assert.NotNull(userUpdate.Id);
            Assert.NotEmpty(userUpdate.Id);

            Assert.NotEqual(userRequestAdd.FirstName, userUpdate.FirstName);
            Assert.NotEqual(userRequestAdd.LastName, userUpdate.LastName);

            var userData = await _userService.GetUserByIdAsync(userId);

            Assert.NotNull(userData);

            bool isDelete = false;

            try
            {
                await _userService.DeleteUserAsync(userId, "System_Unit_Test");
                await _userService.DeleteUserAsync(userId02, "System_Unit_Test");

                isDelete = true;
            }
            catch
            {

            }

            Assert.True(isDelete);
        }
    }
}