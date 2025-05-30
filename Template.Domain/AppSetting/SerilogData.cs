namespace Template.Domain.AppSetting
{
    public class SerilogData
    {
        public List<string>? Using { get; set; }
        public minimumLevel? MinimumLevel { get; set; }
        public List<writeTo>? WriteTo { get; set; }
        public List<string>? Enrich { get; set; }
        public properties? Properties { get; set; }

        public class minimumLevel
        {
            public string? Default { get; set; }
            public Override? Override { get; set; }
            public string? MicrosoftHostingLifetime { get; set; }
        }

        public class Override
        {
            public string? MicrosoftAspNetCore { get; set; }
        }

        public class writeTo
        {
            public string? Name { get; set; }
            public Args? Args { get; set; }
        }

        public class Args
        {
            public string? path { get; set; }
            public string? rollingInterval { get; set; }
        }

        public class properties
        {
            public string? ApplicationName { get; set; }
        }
    }
}