using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Template.Domain.DTO
{
    public class EmailDTO
    {
        public EmailType EmailType { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Required(ErrorMessage = "Subject not empty.")]
        public string? Subject { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Required(ErrorMessage = "Body not empty.")]
        public string? Body { get; set; }

        public BodyType BodyType { get; set; }

        public List<SendTo>? SendTo { get; set; }

        public List<CcTo>? CcTo { get; set; }
    }

    public class SendTo
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
    public class CcTo
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
    }

    public enum BodyType
    {
        HTML = 1,
        TEXT = 2
    }

    public enum EmailType
    {
        SMTP = 1,
        GRAPH_API = 3
    }
}