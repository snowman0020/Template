using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Template.Infrastructure.Share;

namespace Template.Infrastructure.Models
{
    public class Users : MainFields
    {
        [Key]
        [Required]
        [MaxLength(36)]
        [Description("User id.")]
        public string? ID { get; set; }

        [Required]
        [MaxLength(100)]
        [Description("User firstname.")]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        [Description("User lastname.")]
        public string? LastName { get; set; }

        [Required]
        [MaxLength(20)]
        [Description("User mobile.")]
        public string? Phone { get; set; }

        [Required]
        [MaxLength(50)]
        [Description("User email.")]
        public string? Email { get; set; }

        [Required]
        [MaxLength(100)]
        [Description("Password encrypt.")]
        public string? Password { get; set; }
    }
}