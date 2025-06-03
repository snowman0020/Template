namespace Template.Domain.AppSetting
{
    public class CustomSettingData
    {
        public string? MainUrl { get; set; }
        public string? AllowedHosts { get; set; }
        public string? ConnectionSQLServer { get; set; }
        public string? ConnectionMySQLServer { get; set; }
        public string? ConnectionOracleServer { get; set; }
        public string? ConnectionSQLiteServer { get; set; }
        public string? ConnectionPostgreSQLServer { get; set; }
        public MongoDB? MongoDB { get; set; }
    }

    public class MongoDB
    {
        public string? ConnectionServer { get; set; }
        public string? DatabaseName { get; set; }
    }
}