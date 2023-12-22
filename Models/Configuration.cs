namespace CappuAngDiscordBot.Models;

public record Configuration
{
    public required string Token { get; init; }
    public required string[] Prefixes { get; init; }
    public required ulong GuildId { get; init; }
}
