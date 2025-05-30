using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Infrastructure.Models
{
    public class Users
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