using Template.Domain.DTO;

namespace Template.Helper
{
    public interface IErrorExceptionHandler
    {
        ErrorResultDTO ErrorException(Exception exception, ErrorStatus errorStatus, string? title, string? message);
    }
}