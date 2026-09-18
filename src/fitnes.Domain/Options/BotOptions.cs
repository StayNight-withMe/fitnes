namespace fitnes.Domain.Options;

public class BotOptions
{
    public const string SectionName = "TelegramBot";
    public required string Token { get; set; }
}
