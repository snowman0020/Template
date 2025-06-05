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
        public ObjectId _id { get; set; }

        [Required]
        [MaxLength(36)]
        [Description("Message id.")]
        public string? ID { get; set; }

        [Required]
        [MaxLength(36)]
        [Description("User id (FK).")]
        public string? UserID { get; set; }

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