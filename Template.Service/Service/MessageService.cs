using Microsoft.Extensions.Logging;
using System.Text.Json;
using Template.Domain.DTO;
using Template.Helper.ErrorException;
using Template.Infrastructure;
using Template.Infrastructure.Models;
using Template.Service.IServices;

namespace Template.Service.Services
{
    public class MessageService : IMessageService
    {
        private readonly TemplateDbContext _db;
        private readonly ILogger<MessageService> _logger;

        public MessageService(TemplateDbContext db, ILogger<MessageService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task AddMessageAsync(MessageDTO input)
        {
            _logger.LogInformation($"call: AddMessageAsync");

            _logger.LogDebug($"Message add: {JsonSerializer.Serialize(input)}");

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var message = new Messages();

                    input.AddToModel(message);

                    await _db.Messages.AddAsync(message);
                    await _db.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (ErrorException)
                {
                    transaction.Rollback();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    Error.Status = ErrorStatus.BAD_REQUEST;
                    Error.Title = "Can not add data.";
                    Error.Message = ex.Message.ToString();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw new ErrorException();
                }
            }
        }
    }
}