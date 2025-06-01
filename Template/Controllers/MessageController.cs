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
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IErrorExceptionHandler _errorExceptionHandler;

        public MessageController(IMessageService messageService, IErrorExceptionHandler errorExceptionHandler)
        {
            _messageService = messageService;
            _errorExceptionHandler = errorExceptionHandler;
        }

        /// <summary>
        /// Add Message
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Message data</returns>
        /// <response code="200">Returns 200 if success</response>
        /// <response code="400">Returns 400 if error or can not add</response>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AddMessageAsync([Required][FromBody] MessageDTO input)
        {
            try
            {
                await _messageService.AddMessageAsync(input);
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