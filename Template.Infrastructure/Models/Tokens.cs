using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Infrastructure.Models
{
    public class Tokens
    {
        [Key]
        [Required]
        [MaxLength(300)]
        [Description("Token.")]
        public string? Token { get; set; }

        [Required]
        [MaxLength(50)]
        [Description("Refresh token.")]
        public string? RefreshToken { get; set; }

        [Required]
        [MaxLength(50)]
        [Description("Email.")]
        public string? Email { get; set; }

        [Required]
        [Description("Expires.")]
        public double Expires { get; set; }
        
        [Required]
        [Description("Token expire data.")]
        public DateTime ExpiredDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Description("Record No.")]
        public int OrderNumber { get; set; }

        [Description("Delete yes or not.")]
        public bool IsDeleted { get; set; }

        [Description("Create date.")]
        public DateTime CreatedDate { get; set; }

        [MaxLength(200)]
        [Description("Create name.")]
        public string? CreatedBy { get; set; }

        [Description("Update date.")]
        public DateTime? UpdatedDate { get; set; }

        [MaxLength(200)]
        [Description("Update name.")]
        public string? UpdatedBy { get; set; }

        [Description("Delete date.")]
        public DateTime? DeletedDate { get; set; }

        [MaxLength(200)]
        [Description("Delete name.")]
        public string? DeletedBy { get; set; }
    }
}