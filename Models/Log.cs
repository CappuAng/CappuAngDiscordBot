namespace CappuAng.DiscordBot.Models;

public record Log
{
    public DateTime DateTime { get; init; }
    public string? Message { get; init; }
    public LogLevel Level { get; init; }
}
