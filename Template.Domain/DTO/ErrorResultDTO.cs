namespace Template.Domain.DTO
{
    public class ErrorResultDTO
    {
        public int? Status { get; set; }
        public string? Type { get; set; }
        public string? Title { get; set; }
        public string? Detail { get; set; }
        //public string? Instance { get; set; }
    }

    public enum ErrorStatus
    {
        BAD_REQUEST = 400,
        UN_AUTHORIZED = 401,
        FORBIDDEN = 403,
        NOT_FOUND = 404,
        METHOD_NOT_ALLOWED = 405,
        INTERNAL_SERVER_ERROR = 500
    }
}