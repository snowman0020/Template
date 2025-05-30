using Template.Helper;

namespace Template.UnitTest
{
    public class PasswordHashUnitTest
    {
        [Fact]
        public void PasswordEncryptTest()
        {
            string password = "123455678";
            var passwordEncrypt = PasswordHash.Encrypt(password);

            Assert.NotEmpty(passwordEncrypt);
            Assert.NotNull(passwordEncrypt);
            Assert.NotEqual(password, passwordEncrypt);
        }

        [Fact]
        public void PasswordEncryptVerifyTest()
        {
            string password = "123455678";
            var passwordEncrypt = PasswordHash.Encrypt(password);

            Assert.NotEmpty(passwordEncrypt);
            Assert.NotNull(passwordEncrypt);
            Assert.NotEqual(password, passwordEncrypt);

            var passwordVerify = PasswordHash.Verify(passwordEncrypt, password);

            Assert.True(passwordVerify);
        }
    }
}