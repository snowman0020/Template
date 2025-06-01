using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Template.Infrastructure.Share
{
    public class MainFields
    {

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