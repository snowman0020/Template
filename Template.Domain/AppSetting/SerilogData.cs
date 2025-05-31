namespace Template.Domain.AppSetting
{
    public class SerilogData
    {
        public List<string>? Using { get; set; }
        public MinimumLevelData? MinimumLevel { get; set; }
        public List<WriteToData>? WriteTo { get; set; }
        public List<string>? Enrich { get; set; }
        public PropertiesData? Properties { get; set; }

        public class MinimumLevelData
        {
            public string? Default { get; set; }
            public Override? Override { get; set; }
            public string? MicrosoftHostingLifetime { get; set; }
        }

        public class Override
        {
            public string? MicrosoftAspNetCore { get; set; }
        }

        public class WriteToData
        {
            public string? Name { get; set; }
            public Args? Args { get; set; }
        }

        public class Args
        {
            public string? path { get; set; }
            public string? rollingInterval { get; set; }
        }

        public class PropertiesData
        {
            public string? ApplicationName { get; set; }
        }
    }
}