using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using Template.Domain.DTO;
using Template.Domain.Filter;
using Template.Domain.Paging;
using Template.Domain.SortBy;
using Template.Helper.ErrorException;
using Template.Service.IServices;

namespace Template.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IErrorExceptionHandler _errorExceptionHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private List<Claim> _userClaimsList = new List<Claim>();

        public UserController(IUserService userService, IErrorExceptionHandler errorExceptionHandler, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _errorExceptionHandler = errorExceptionHandler;
            _httpContextAccessor = httpContextAccessor;

            if (_httpContextAccessor.HttpContext != null)
            {
                _userClaimsList = _httpContextAccessor.HttpContext.User.Claims.ToList();
            }
        }

        /// <summary>
        /// Get user list
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pageParam"></param>s
        /// <param name="sortBy"></param>
        /// <returns>User List and paging</returns>
        /// <response code="200">Returns 200 if success</response>
        /// <response code="400">Returns 400 if error or not found</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
                var userId = _userClaimsList[0].Value;

                var result = await _userService.GetUserByIdAsync(userId);
                return Ok(result);
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
                var userAddId = _userClaimsList[0].Value;

                var result = await _userService.AddUserAsync(input, userAddId);
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
                var userUpdateId = _userClaimsList[0].Value;

                var result = await _userService.UpdateUserAsync(id, input, userUpdateId);
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
                var userDeleteId = _userClaimsList[0].Value;

                await _userService.DeleteUserAsync(id, userDeleteId);
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