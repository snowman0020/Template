using System.Text.Json;
using Template.Infrastructure.Models;

namespace Template.Domain.DTO
{
    public class LineRequest
    {
        public string? to { get; set; }
        public List<MessagesData>? messages { get; set; }
    }
    public class LineResponse
    {
        public string? message { get; set; }
        public List<SentMessages>? sentMessages { get; set; }
    }


    public class MessagesData
    {
        public string? type { get; set; }
        public string? text { get; set; }
    }

    public class LineDTO
    {
        public string? message { get; set; }
        public List<SentMessages>? sentMessages { get; set; }
        public bool isSentSuccess { get; set; }
        public DateTime sentSuccessDate { get; set; }
        public void AddToModel(MessageLines model, string messageId)
        {
            model.ID = Guid.NewGuid().ToString();
            model.MessageID = messageId;
            model.IsSentSuccess = isSentSuccess;
            model.SentSuccessDate = sentSuccessDate;
            model.MessageError = message;
            model.SentMessage = sentMessages != null ? JsonSerializer.Serialize(sentMessages) : "";
        }

        public void UpdateToModel(MessageLines model, string messageId)
        {
            model.MessageID = messageId;
            model.IsSentSuccess = isSentSuccess;
            model.SentSuccessDate = sentSuccessDate;
            model.MessageError = message;
            model.SentMessage = sentMessages != null ? JsonSerializer.Serialize(sentMessages) : "";
        }
    }

    public class SentMessages
    {
        public string? id { get; set; }
        public string? quoteToken { get; set; }
    }
    }