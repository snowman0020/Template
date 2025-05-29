using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
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

                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageData));
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                var errorExceptionResult = _errorExceptionHandler.ErrorException(ex, Error.Status, Error.Title, Error.Message);
                return new ObjectResult(errorExceptionResult) { StatusCode = errorExceptionResult.Status };
            }
        }

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