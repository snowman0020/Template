using Microsoft.EntityFrameworkCore;
using Template.Domain.DTO;
using Template.Helper.Token;
using Template.Infrastructure;
using Template.Service.IServices;

namespace Template.UnitTest.Test
{
    public class TokenUnitTest : IClassFixture<Startup>, IDisposable
    {
        private readonly IUserService _userService;
        private readonly TemplateDbContext _db;
        private readonly IToken _token;

        public TokenUnitTest(IUserService userService, TemplateDbContext db, IToken token)
        {
            _userService = userService;
            _db = db;
            _token = token;
        }

        public void Dispose()
        {
            _db.Database.EnsureDeleted();
            _db.Database.Migrate();
        }

        [Fact]
        public async Task CreateNewTokenTest()
        {
            Dispose();

            string userId = "";
            string email = "";

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
            Assert.NotNull(userAdd.Email);
            Assert.NotEmpty(userAdd.Email);

            userId = userAdd.Id;
            email = userAdd.Email;

            string key = "eqbqRp2AhMCy88PAVR9RYa2zjAPfwz1KD9z6eGFkOWOXustez7sUTr9CWViEY9Ig,";
            int expMinutes = 15;
            string issuer = "http://localhost:5011";

            var createNewToken = _token.CreateNewToken(userId, email, issuer, expMinutes, key);

            Assert.NotNull(createNewToken);
            Assert.NotNull(createNewToken.Token);
            Assert.NotEmpty(createNewToken.Token);
            Assert.NotNull(createNewToken.RefreshToken);
            Assert.NotEmpty(createNewToken.RefreshToken);

            bool isDelete = false;

            try
            {
                await _userService.DeleteUserAsync(userId, "System_Unit_Test");

                isDelete = true;
            }
            catch
            {
              
            }

            Assert.True(isDelete);
        }
    }
}