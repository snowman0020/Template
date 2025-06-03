namespace Template.Domain.AppSetting
{
    public class EmailData
    {
        public Smtp? Smtp { get; set; }
        public GraphApi? GraphApi { get; set; }
    }

    public class Smtp
    {
        public string? Server { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool PasswordEnable { get; set; }
    }

    public class GraphApi
    {
        public string? TenantId { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? PrincipalId { get; set; }
        public string? PrincipalName { get; set; }
        public string? Url { get; set; }
    }
}