namespace Template.Domain.AppSetting
{
    public class LoggingData
    {
        public LogLevelData? LogLevel { get; set; }
        public string? Type { get; set; }
        public string? Title { get; set; }
        public string? Detail { get; set; }

        public class LogLevelData
        {
            public string? Default { get; set; }
            public string? Microsoft { get; set; }
            public string? MicrosoftHostingLifetime { get; set; }
        }
    }
}