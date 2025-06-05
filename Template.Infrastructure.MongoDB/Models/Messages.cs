using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Template.Infrastructure.MongoDB.Share;

namespace Template.Infrastructure.MongoDB.Models
{
    [Collection("Messages")]
    public class Messages : MainFields
    {
        [Required]
        [Description("Message id.")]
        public ObjectId ID { get; set; }

        [Required]
        [Description("User id (FK).")]
        public ObjectId? UserID { get; set; }

        [Required]
        [MaxLength(100)]
        [Description("Topic.")]
        public string? Topic { get; set; }

        [Required]
        [MaxLength(300)]
        [Description("Detail.")]
        public string? Detail { get; set; }

        [Description("Sent message yes or not.")]
        public bool IsSent { get; set; }

        [Description("Sent date.")]
        public DateTime SentDate { get; set; }

        [MaxLength(200)]
        [Description("Sent name.")]
        public string? SentBy { get; set; }
    }
}