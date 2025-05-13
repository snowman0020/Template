using System.ComponentModel.DataAnnotations;

namespace Template.Infrastructure.Models
{
    public class Users
    {
        [Key]
        [MaxLength(36)]
        public string? ID { get; set; }
        [MaxLength(10)]
        public string? Username { get; set; }
        [MaxLength(100)]
        public string? FirstName { get; set; }
        [MaxLength(100)]
        public string? LastName { get; set; }
        [MaxLength(20)]
        public string? Phone { get; set; }
        [MaxLength(50)]
        public string? Email { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        [MaxLength(200)]
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [MaxLength(200)]
        public string? UpdatedBy { get; set; }
    }
}
