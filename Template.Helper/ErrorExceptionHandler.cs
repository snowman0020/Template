using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net;
using System.Security.Authentication;
using Template.Domain.DTO;

namespace Template.Helper
{
    public class ErrorExceptionHandler : IErrorExceptionHandler
    {
        private readonly ILogger<ErrorExceptionHandler> _logger;
        public ErrorExceptionHandler(ILogger<ErrorExceptionHandler> logger)
        {
            _logger = logger;
        }

        public ErrorResultDTO ErrorException(Exception exception, ErrorStatus errorStatus, string? title, string? message)
        {
            var result = new ErrorResultDTO();

           //_logger.LogError(exception, "An unexpected error occurred");

            int status = 0;
            string type = "";

            switch (errorStatus)
            {
                case ErrorStatus.BAD_REQUEST:
                    status = (int)ErrorStatus.BAD_REQUEST;
                    type = "Bad_Request";
                    break;
                case ErrorStatus.UN_AUTHORIZED:
                    status = (int)ErrorStatus.UN_AUTHORIZED;
                    type = "Un_authorized";
                    break;
                case ErrorStatus.FORBIDDEN:
                    status = (int)ErrorStatus.FORBIDDEN;
                    type = "Forbidden";
                    break;
                case ErrorStatus.NOT_FOUND:
                    status = (int)ErrorStatus.NOT_FOUND;
                    type = "Not_Found";
                    break;
                case ErrorStatus.METHOD_NOT_ALLOWED:
                    status = (int)ErrorStatus.METHOD_NOT_ALLOWED;
                    type = "Method_Not_Allowed";
                    break;
                case ErrorStatus.INTERNAL_SERVER_ERROR:
                    status = (int)ErrorStatus.INTERNAL_SERVER_ERROR;
                    type = "Internal_Server_Error";
                    break;
            }

            result.Status = status;
            result.Type = type;
            result.Title = title;
            result.Detail = message;
            //result.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";

            return result;
        }
    }
}
