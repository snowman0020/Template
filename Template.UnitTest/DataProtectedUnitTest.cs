using Template.Helper.DataProtected;

namespace Template.UnitTest
{
    public class DataProtectedUnitTest : IClassFixture<Startup>
    {
        private readonly IDataProtected _dataProtected;

        public DataProtectedUnitTest(IDataProtected dataProtected)
        {
            _dataProtected = dataProtected;
        }

        [Fact]
        public void ProtectInformationTest()
        {
            string sensitiveData = "Test Information.";
            var protectedData = _dataProtected.ProtectInformation(sensitiveData);

            Assert.NotEmpty(protectedData);
            Assert.NotNull(protectedData);
            Assert.NotEqual(sensitiveData, protectedData);
        }

        [Fact]
        public void UnProtectInformationTest()
        {
            string sensitiveData = "Test Information.";
            var protectedData = _dataProtected.ProtectInformation(sensitiveData);

            Assert.NotEmpty(protectedData);
            Assert.NotNull(protectedData);
            Assert.NotEqual(sensitiveData, protectedData);
            
            var unProtectedData = _dataProtected.UnProtectInformantion(protectedData);

            Assert.NotEmpty(unProtectedData);
            Assert.NotNull(unProtectedData);
            Assert.Equal(sensitiveData, unProtectedData);
        }
    }
}