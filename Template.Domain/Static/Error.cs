using static Template.Domain.DTO.ErrorResultDTO;

namespace Template.Domain.DTO
{
    public static class Error
    {
        public static ErrorStatus Status { get; set; }
        public static string? Title { get; set; }
        public static string? Message { get; set; }
    }
}
