namespace MyService.TestAPI
{
    public class Logging
    {
        public string Debug { get; set; }
        public LogLevel LogLevel { get; set; }
    }

    public class LogLevel
    {
        public string Default { get; set; }
    }

    public class AppConfigSettings
    {
        public string Endpoint { get; set; }
    }
}
