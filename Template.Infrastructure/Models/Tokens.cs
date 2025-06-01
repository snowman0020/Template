using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Template.Infrastructure.Share;

namespace Template.Infrastructure.Models
{
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