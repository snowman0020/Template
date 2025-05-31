using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Template.Domain.DTO;
using Template.Helper.ErrorException;
using Template.Service.IServices;

namespace Template.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IErrorExceptionHandler _errorExceptionHandler;

        public TokenController(ITokenService tokenService, IErrorExceptionHandler errorExceptionHandler)
        {
            _tokenService = tokenService;
            _errorExceptionHandler = errorExceptionHandler;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Token data</returns>
        /// <response code="200">Returns 200 if success</response>
        /// <response code="400">Returns 400 if error or not found</response>
        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(200, Type = typeof(LoginDTO))]
        public async Task<IActionResult> LoginAsync([Required][FromBody] LoginRequest input)
        {
            try
            {
                var result = await _tokenService.LoginAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var errorExceptionResult = _errorExceptionHandler.ErrorException(ex, Error.Status, Error.Title, Error.Message);
                return new ObjectResult(errorExceptionResult) { StatusCode = errorExceptionResult.Status };
            }
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <response code="200">Returns 200 if success</response>
        /// <response code="400">Returns 400 if error or not found</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Logout")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> LogoutAsync([Required][FromBody] LogoutRequest input)
        {
            try
            {
                await _tokenService.LogoutAsync(input);
                return Ok();
            }
            catch (Exception ex)
            {
                var errorExceptionResult = _errorExceptionHandler.ErrorException(ex, Error.Status, Error.Title, Error.Message);
                return new ObjectResult(errorExceptionResult) { StatusCode = errorExceptionResult.Status };
            }
        }
    }
}