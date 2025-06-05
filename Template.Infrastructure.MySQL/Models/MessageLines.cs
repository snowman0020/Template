using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Template.Infrastructure.MySQL.Share;

namespace Template.Infrastructure.MySQL.Models
{
    public class MessageLines : MainFields
    {
        [Key]
        [Required]
        [MaxLength(36)]
        [Description("Message line id.")]
        public string? ID { get; set; }

        [Required]
        [MaxLength(36)]
        [Description("Message id (FK).")]
        public string? MessageID { get; set; }

        [ForeignKey("MessageID")]
        public virtual Messages? Messages { get; set; }

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