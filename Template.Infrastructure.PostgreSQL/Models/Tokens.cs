using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Template.Infrastructure.PostgreSQL.Share;

namespace Template.Infrastructure.PostgreSQL.Models
{
    [Table("Tokens", Schema = "public")]
    public class Tokens : MainFields
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
    }
}