using Microsoft.EntityFrameworkCore;
using Template.Domain.DTO;
using Template.Infrastructure;
using Template.Service.IServices;

namespace Template.UnitTest
{
    public class TokenServiceUnitTest : IClassFixture<Startup>, IDisposable
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly TemplateDbContext _db;

        public TokenServiceUnitTest(IUserService userService, ITokenService tokenService, TemplateDbContext db)
        {
            _userService = userService;
            _tokenService = tokenService;
            _db = db;
        }

        public void Dispose()
        {
            _db.Database.EnsureDeleted();
            _db.Database.Migrate();
        }

        [Fact]
        public async Task CreateNewTokenFlowTest()
        {
            Dispose();

            string userId = "";
            string refreshToken = "";

            var userRequestAdd = new UserRequest()
            {
                FirstName = "TestFirstName1",
                LastName = "TestLastName1",
                Phone = "0999999999",
                Email = "testFirstName@email.com",
                Password = "1234"
            };

            var userAdd = await _userService.AddUserAsync(userRequestAdd);

            Assert.NotNull(userAdd);
            Assert.NotNull(userAdd.Id);
            Assert.NotEmpty(userAdd.Id);

            userId = userAdd.Id;

            var loginNewTokenRequest = new LoginRequest()
            {
                GrantType = GrantTypeStatus.PASSWORD,
                Email = "testFirstName@email.com",
                Password = "1234"
            };

            var createNewToken = await _tokenService.LoginAsync(loginNewTokenRequest);

            Assert.NotNull(createNewToken);
            Assert.NotNull(createNewToken.Token);
            Assert.NotEmpty(createNewToken.Token);
            Assert.NotNull(createNewToken.RefreshToken);
            Assert.NotEmpty(createNewToken.RefreshToken);

            refreshToken = createNewToken.RefreshToken;

            bool isDelete = false;

            var logtoutRequest = new LogoutRequest()
            {
                RefreshToken = refreshToken
            };

            try
            {
                await _tokenService.LogoutAsync(logtoutRequest);
                await _userService.DeleteUserAsync(userId);

                isDelete = true;
            }
            catch
            {

            }

            Assert.True(isDelete);
        }


        [Fact]
        public async Task CreateNewRefreshToken()
        {
            Dispose();

            string userId = "";
            string refreshToken = "";
            string refreshNewToken = "";

            var userRequestAdd = new UserRequest()
            {
                FirstName = "TestFirstName1",
                LastName = "TestLastName1",
                Phone = "0999999999",
                Email = "testFirstName@email.com",
                Password = "1234"
            };

            var userAdd = await _userService.AddUserAsync(userRequestAdd);

            Assert.NotNull(userAdd);
            Assert.NotNull(userAdd.Id);
            Assert.NotEmpty(userAdd.Id);

            userId = userAdd.Id;

            var loginNewTokenRequest = new LoginRequest()
            {
                GrantType = GrantTypeStatus.PASSWORD,
                Email = "testFirstName@email.com",
                Password = "1234"
            };

            var createNewToken = await _tokenService.LoginAsync(loginNewTokenRequest);

            Assert.NotNull(createNewToken);
            Assert.NotNull(createNewToken.Token);
            Assert.NotEmpty(createNewToken.Token);
            Assert.NotNull(createNewToken.RefreshToken);
            Assert.NotEmpty(createNewToken.RefreshToken);

            refreshToken = createNewToken.RefreshToken;

            var loginNewRefreshTokenRequest = new LoginRequest()
            {
                GrantType = GrantTypeStatus.REFRESH_TOKEN,
                RefreshToken = refreshToken
            };

            var createNewRefreshToken = await _tokenService.LoginAsync(loginNewRefreshTokenRequest);

            Assert.NotNull(createNewRefreshToken);
            Assert.NotNull(createNewRefreshToken.Token);
            Assert.NotEmpty(createNewRefreshToken.Token);
            Assert.NotNull(createNewRefreshToken.RefreshToken);
            Assert.NotEmpty(createNewRefreshToken.RefreshToken);

            Assert.NotEqual(createNewToken.RefreshToken, createNewRefreshToken.RefreshToken);

            refreshNewToken = createNewRefreshToken.RefreshToken;

            bool isDelete = false;

            var logtoutRequest = new LogoutRequest()
            {
                RefreshToken = refreshNewToken
            };

            try
            {
                await _tokenService.LogoutAsync(logtoutRequest);
                await _userService.DeleteUserAsync(userId);

                isDelete = true;
            }
            catch
            {

            }

            Assert.True(isDelete);
        }
    }
}