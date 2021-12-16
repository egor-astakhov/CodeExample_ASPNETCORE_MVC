namespace Integral.Application.Common.Configuration
{
    public class AppSettings
    {
        public PathsSettings Paths { get; set; }

        public class PathsSettings
        {
            public string ExternalWebRoot { get; set; }

            public string BackupRoot { get; set; }
        }
    }
}
