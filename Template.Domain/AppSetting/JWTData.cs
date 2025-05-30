namespace Template.Domain.AppSetting
{
    public class JWTData
    {
        public string? Key { get; set; }
        public int? ExpMinutes { get; set; }
        public string? Issuer { get; set; }
    }
}