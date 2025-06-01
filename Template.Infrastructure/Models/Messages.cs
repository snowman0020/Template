using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Template.Infrastructure.Share;

namespace Template.Infrastructure.Models
{
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
    }
}