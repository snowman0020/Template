using Template.Domain.DTO;
using Template.Helper;
using Template.Service.IServices;

namespace Template.UnitTest
{
    //public class Startup
    //{
    //    public void ConfigureServices(IServiceCollection services)
    //    {
    //        services.AddTransient<IErrorExceptionHandler, ErrorExceptionHandler>();
    //    }
    //}

    public class ErrorExceptionUnitTests : IClassFixture<Startup>
    {
        private readonly IErrorExceptionHandler _errorExceptionHandler;

        public ErrorExceptionUnitTests(IErrorExceptionHandler errorExceptionHandler)
        {
            _errorExceptionHandler = errorExceptionHandler;
        }

        [Fact]
        public void ErrorExceptionTest()
        {
            var exception = new ErrorException();
            var errorStatus = ErrorStatus.BAD_REQUEST;
            var title = "Error title.";
            var message = "Error message.";

            var exceptionResult = _errorExceptionHandler.ErrorException(exception, errorStatus, title, message);

            Assert.NotNull(exceptionResult);
        }
    }
}