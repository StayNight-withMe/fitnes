namespace fitnes.Domain.Constants.Bot;

public static class LoggingConstants
{
    public const string LogDirectory = "logs";
    public const string LogFileName = "bot-.log";
    public const int RetainedFileCount = 7;
    public const string ConsoleOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
    public const string FileOutputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";
}
