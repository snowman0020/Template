using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Template.Infrastructure.MongoDB.Share;

namespace Template.Infrastructure.MongoDB.Models
{
    [Collection("MessageLines")]
    public class MessageLines : MainFields
    {
        [Required]
        [Description("Message line id.")]
        public ObjectId ID { get; set; }

        [Required]
        [Description("Message id (FK).")]
        public ObjectId MessageID { get; set; }

        [Description("Sent message success yes or not.")]
        public bool IsSentSuccess { get; set; }

        [Description("Sent message success date.")]
        public DateTime SentSuccessDate { get; set; }

        [MaxLength(200)]
        [Description("message error.")]
        public string? MessageError { get; set; }

        [MaxLength(1000)]
        [Description("Sent message data.")]
        public string? SentMessage { get; set; }
    }
}