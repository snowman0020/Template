using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Template.Domain.DTO;
using Template.Domain.Filter;
using Template.Domain.Paging;
using Template.Domain.SortBy;
using Template.Service.IServices;

namespace Template.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService UserService;

        public UserController(IUserService userService)
        {
            UserService = userService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(PageList<UserDTO>))]
        public async Task<IActionResult> GetUserListAsync([FromQuery] UserFilter filter, [FromQuery] PageParam pageParam, [FromQuery] UserSortBy sortBy)
        {
            var result = await UserService.GetUserListAsync(filter, pageParam, sortBy);

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

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(UserDTO))]
        public async Task<IActionResult> GetUserByIdAsync([Required][FromRoute] string id)
        {
            var result = await UserService.GetUserByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(UserDTO))]
        public async Task<IActionResult> AddUserAsync([Required][FromBody] UserRequest input)
        {
            try
            {
                var result = await UserService.AddUserAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(UserDTO))]
        public async Task<IActionResult> UpdateUserAsync([Required][FromRoute] string id, [Required][FromBody] UserRequest input)
        {
            try
            {
                var result = await UserService.UpdateUserAsync(id, input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteUserAsync([Required][FromRoute] string id)
        {
            try
            {
                await UserService.DeleteUserAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
