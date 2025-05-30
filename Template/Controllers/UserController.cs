using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Template.Domain.DTO;
using Template.Domain.Filter;
using Template.Domain.Paging;
using Template.Domain.SortBy;
using Template.Helper;
using Template.Service.IServices;

namespace Template.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IErrorExceptionHandler _errorExceptionHandler;

        public UserController(IUserService userService, IErrorExceptionHandler errorExceptionHandler)
        {
            _userService = userService;
            _errorExceptionHandler = errorExceptionHandler;
        }

        /// <summary>
        /// Get user list
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pageParam"></param>
        /// <param name="sortBy"></param>
        /// <returns>User List and paging</returns>
        /// <response code="200">Returns 200 if success</response>
        /// <response code="400">Returns 400 if error or not found</response>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(PageList<UserDTO>))]
        public async Task<IActionResult> GetUserListAsync([FromQuery] UserFilter filter, [FromQuery] PageParam pageParam, [FromQuery] UserSortBy sortBy)
        {
            try
            {
                var result = await _userService.GetUserListAsync(filter, pageParam, sortBy);

                if (result != null && result.Count > 0)
                {
                    var pageData = new
                    {
                        result.TotalCount,
                        result.PageSize,
                        result.CurrentPage,
                        result.TotalPages,
                        result.HasNext,
                        result.HasPrevious
                    };

                    Response.Headers["X-Pagination"] = JsonSerializer.Serialize(pageData);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                var errorExceptionResult = _errorExceptionHandler.ErrorException(ex, Error.Status, Error.Title, Error.Message);
                return new ObjectResult(errorExceptionResult) { StatusCode = errorExceptionResult.Status };
            }
        }

        /// <summary>
        /// Get user by me
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User data</returns>
        /// <response code="200">Returns 200 if success</response>
        /// <response code="400">Returns 400 if error or not found</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Me")]
        [ProducesResponseType(200, Type = typeof(UserDTO))]
        public async Task<IActionResult> GetUserByMeAsync()
        {
            try
            {
                var userClaimsList = HttpContext.User.Claims.ToList();

                var userId = userClaimsList[0].Value;

                var result = await _userService.GetUserByIdAsync(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                var errorExceptionResult = _errorExceptionHandler.ErrorException(ex, Error.Status, Error.Title, Error.Message);
                return new ObjectResult(errorExceptionResult) { StatusCode = errorExceptionResult.Status };
            }
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User data</returns>
        /// <response code="200">Returns 200 if success</response>
        /// <response code="400">Returns 400 if error or not found</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(UserDTO))]
        public async Task<IActionResult> GetUserByIdAsync([Required][FromRoute] string id)
        {
            try
            {
                var result = await _userService.GetUserByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var errorExceptionResult = _errorExceptionHandler.ErrorException(ex, Error.Status, Error.Title, Error.Message);
                return new ObjectResult(errorExceptionResult) { StatusCode = errorExceptionResult.Status };
            }
        }

        /// <summary>
        /// Add user
        /// </summary>
        /// <param name="input"></param>
        /// <returns>User data</returns>
        /// <response code="200">Returns 200 if success</response>
        /// <response code="400">Returns 400 if error or can not add</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(UserDTO))]
        public async Task<IActionResult> AddUserAsync([Required][FromBody] UserRequest input)
        {
            try
            {
                var result = await _userService.AddUserAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var errorExceptionResult = _errorExceptionHandler.ErrorException(ex, Error.Status, Error.Title, Error.Message);
                return new ObjectResult(errorExceptionResult) { StatusCode = errorExceptionResult.Status };
            }
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns>User data</returns>
        /// <response code="200">Returns 200 if success</response>
        /// <response code="400">Returns 400 if error or can not update</response>
        [AllowAnonymous]
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(UserDTO))]
        public async Task<IActionResult> UpdateUserAsync([Required][FromRoute] string id, [Required][FromBody] UserRequest input)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(id, input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var errorExceptionResult = _errorExceptionHandler.ErrorException(ex, Error.Status, Error.Title, Error.Message);
                return new ObjectResult(errorExceptionResult) { StatusCode = errorExceptionResult.Status };
            }
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Returns 200 if success</response>
        /// <response code="400">Returns 400 if error or can not delete</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteUserAsync([Required][FromRoute] string id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
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