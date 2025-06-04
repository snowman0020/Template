using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Template.Domain.AppSetting;
using Template.Domain.DTO;
using Template.Helper.ErrorException;
using Template.Helper.Line;
using Template.Infrastructure;
using Template.Infrastructure.Models;
using Template.Service.IServices;

namespace Template.Service.Services
{
    public class MessageService : IMessageService
    {
        private readonly TemplateDbContext _db;
        private readonly ILogger<MessageService> _logger;
        private readonly ILine _line;
        private readonly LineData _lineData;

        public MessageService(TemplateDbContext db, ILogger<MessageService> logger, ILine line, IOptions<LineData> lineData)
        {
            _db = db;
            _logger = logger;
            _line = line;
            _lineData = lineData.Value;
        }

        public async Task AddMessageAsync(MessageDTO input)
        {
            _logger.LogInformation($"call: AddMessageAsync=> Start");

            var message = new Messages();

            using (var transaction = _db.Database.BeginTransaction())
            {
                bool isMessageAddSuccess = false;

                try
                {
                    _logger.LogDebug($"Message add: {JsonSerializer.Serialize(input)}");

                    input.AddToModel(message);

                    await _db.Messages.AddAsync(message);
                    await _db.SaveChangesAsync();

                    isMessageAddSuccess = true;

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
                finally
                {
                    if (isMessageAddSuccess)
                    {
                        _logger.LogDebug($"isMessageAddSuccess: {isMessageAddSuccess}");

                        string userId = _lineData.MessageUserId ?? "";
                        string type = _lineData.MessageType ?? "";

                        var lineRequest = new LineRequest();
                        lineRequest.to = userId;

                        var messagesData = new MessagesData();
                        messagesData.type = type;

                        var baseDirectory = AppContext.BaseDirectory;
                        string messageHtml = File.ReadAllText(Path.Combine(baseDirectory, "Message.html"));
                        messageHtml = messageHtml.Replace("{topic}", message.Topic);
                        messageHtml = messageHtml.Replace("{detail}", message.Detail);
                        messageHtml = messageHtml.Replace("{user}", message.ID);
                        messageHtml = messageHtml.Replace("{createDate}", message.CreatedDate.ToString());

                        messagesData.text = messageHtml;

                        var messages = new List<MessagesData>();
                        messages.Add(messagesData);

                        lineRequest.messages = messages;

                        var result = await _line.SendMessageAsync(lineRequest);

                        _logger.LogDebug($"data: {JsonSerializer.Serialize(result)}");

                        string messageId = message.ID ?? "";

                        await UpdateMessageAsync(result, messageId);
                    }
                }

                _logger.LogInformation($"call: AddMessageAsync=> Finish");
            }
        }

        private async Task UpdateMessageAsync(LineDTO input, string messageId)
        {
            _logger.LogInformation($"call: UpdateMessageAsync=> Start");

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _logger.LogDebug($"Message update: {JsonSerializer.Serialize(input)}, message id: {messageId}");

                    var messageLine = new MessageLines();

                    input.AddToModel(messageLine, messageId);

                    await _db.MessageLines.AddAsync(messageLine);
                    await _db.SaveChangesAsync();

                    if (input.isSentSuccess)
                    {
                        var modelMessage = await _db.Messages.Where(m => m.ID == messageId).FirstOrDefaultAsync();

                        if (modelMessage != null)
                        {
                            modelMessage.IsSent = input.isSentSuccess;
                            modelMessage.SentDate = input.sentSuccessDate;
                            modelMessage.SentBy = "System";

                            _db.Messages.Update(modelMessage);
                            await _db.SaveChangesAsync();
                        }
                    }

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
                    Error.Title = "Can not update data.";
                    Error.Message = ex.Message.ToString();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw new ErrorException();
                }

                _logger.LogInformation($"call: UpdateMessageAsync=> Finish");
            }
        }

        public async Task CheckMessageAsync()
        {
            _logger.LogInformation($"call: CheckMessageAsync=> Start");

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var modelMessage = await _db.Messages.Where(m => m.IsSent == false && m.IsDeleted == false).ToListAsync();
                    
                    var modelMessageLine = await _db.MessageLines.Where(m => m.IsSentSuccess == false && m.IsDeleted == false).Include(m => m.Messages).ToListAsync();

                    if (modelMessageLine != null && modelMessageLine.Count > 0)
                    {
                        string userId = _lineData.MessageUserId ?? "";
                        string type = _lineData.MessageType ?? "";

                        var lineRequest = new LineRequest();
                        lineRequest.to = userId;

                        var messagesData = new MessagesData();
                        messagesData.type = type;

                        foreach (var message in modelMessageLine)
                        {
                            var baseDirectory = AppContext.BaseDirectory;
                            string messageHtml = File.ReadAllText(Path.Combine(baseDirectory, "Message.html"));
                            messageHtml = messageHtml.Replace("{topic}", message.Messages?.Topic);
                            messageHtml = messageHtml.Replace("{detail}", message.Messages?.Detail);
                            messageHtml = messageHtml.Replace("{user}", message.ID);
                            messageHtml = messageHtml.Replace("{createDate}", message.CreatedDate.ToString());

                            messagesData.text = messageHtml;

                            var messages = new List<MessagesData>();
                            messages.Add(messagesData);

                            lineRequest.messages = messages;

                            var result = await _line.SendMessageAsync(lineRequest);

                            if (result.isSentSuccess)
                            {
                                string messageId = message.ID ?? "";
                                result.UpdateToModel(message, messageId);
                                _db.MessageLines.Update(message);

                                if (modelMessage != null && modelMessage.Count > 0)
                                {
                                   var modelMessageUpdate = modelMessage.Where(m => m.ID == message.MessageID).FirstOrDefault();

                                    if (modelMessageUpdate != null)
                                    {
                                        modelMessageUpdate.IsSent = result.isSentSuccess;
                                        modelMessageUpdate.SentDate = result.sentSuccessDate;
                                        modelMessageUpdate.SentBy = "System";

                                        _db.Messages.Update(modelMessageUpdate);
                                        await _db.SaveChangesAsync();
                                    }
                                }
                            }
                        }

                        transaction.Commit();
                    }
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
                    Error.Title = "Can not check data.";
                    Error.Message = ex.Message.ToString();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw new ErrorException();
                }

                _logger.LogInformation($"call: CheckMessageAsync=> Finish");
            }
        }
    }
}