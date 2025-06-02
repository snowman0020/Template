using Template.Helper.PasswordHash;

namespace Template.UnitTest.Test
{
    public class PasswordHashUnitTest : IClassFixture<Startup>
    {
        private readonly IPasswordHash _passwordHash;

        public PasswordHashUnitTest(IPasswordHash passwordHash)
        {
            _passwordHash = passwordHash;
        }

        [Fact]
        public void PasswordEncryptTest()
        {
            string password = "123455678";
            var passwordEncrypt = _passwordHash.Encrypt(password);

            Assert.NotEmpty(passwordEncrypt);
            Assert.NotNull(passwordEncrypt);
            Assert.NotEqual(password, passwordEncrypt);
        }

        [Fact]
        public void PasswordEncryptVerifyTest()
        {
            string password = "123455678";
            var passwordEncrypt = _passwordHash.Encrypt(password);

            Assert.NotEmpty(passwordEncrypt);
            Assert.NotNull(passwordEncrypt);
            Assert.NotEqual(password, passwordEncrypt);

            var passwordVerify = _passwordHash.Verify(passwordEncrypt, password);

            Assert.True(passwordVerify);
        }
    }
}