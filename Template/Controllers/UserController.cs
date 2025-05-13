using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
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

        [PagingResponseHeaders]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<UserDTO>))]
        public async Task<IActionResult> GetUserListAsync([FromQuery] UserFilter userFilter, [FromQuery] PageParam pageParam, [FromQuery] UserSortBy? userSortBy)
        {
            var result = await UserService.GetUserListAsync(userFilter, pageParam, userSortBy);

            if (result.PageOutput != null)
            {
                Response.Headers.Add("Access-Control-Expose-Headers", "X-Paging-PageNo, X-Paging-PageSize, X-Paging-PageCount, X-Paging-TotalRecordCount");
                Response.Headers.Add("X-Paging-PageNo", result.PageOutput.Page.ToString());
                Response.Headers.Add("X-Paging-PageSize", result.PageOutput.PageSize.ToString());
                Response.Headers.Add("X-Paging-PageCount", result.PageOutput.PageCount.ToString());
                Response.Headers.Add("X-Paging-TotalRecordCount", result.PageOutput.RecordCount.ToString());
            }

            return Ok(result.UserList);
        }

        [HttpGet("{ID}")]
        [ProducesResponseType(200, Type = typeof(UserDTO))]
        public async Task<IActionResult> GetUserByIdAsync([Required][FromRoute] string ID)
        {
            var result = await UserService.GetUserByIdAsync(ID);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(UserDTO))]
        public async Task<IActionResult> CreateUserAsync([Required][FromBody] UserDTO input)
        {
            try
            {
                var result = await UserService.CreateUserAsync(input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPut("{ID}")]
        [ProducesResponseType(200, Type = typeof(UserDTO))]
        public async Task<IActionResult> UpdateUserAsync([Required][FromRoute] string ID, [Required][FromBody] UserDTO input)
        {
            try
            {
                var result = await UserService.UpdateUserAsync(ID, input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpDelete("{ID}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> DeleteUserAsync([Required][FromRoute] string ID)
        {
            try
            {
                var result = await UserService.DeleteUserAsync(ID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
