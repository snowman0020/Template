using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Template.Infrastructure.PostgreSQL.Share;

namespace Template.Infrastructure.PostgreSQL.Models
{
    [Table("Messages", Schema = "public")]
    public class Messages : MainFields
    {
        [Key]
        [Required]
        [MaxLength(36)]
        [Description("Message id.")]
        public string? ID { get; set; }

        [Required]
        [MaxLength(36)]
        [Description("User id (FK).")]
        public string? UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual Users? Users { get; set; }

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