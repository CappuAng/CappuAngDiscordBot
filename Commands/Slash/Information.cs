using CappuAng.DiscordBot.Models;
using Discord.Interactions;

namespace CappuAng.DiscordBot.Commands.Slash;

public class Information : SlashCommand
{
    [SlashCommand("information", "Shows information about the bot.")]
    public override async Task Execute() =>
        await RespondAsync("This is the execution message for the Information command.");
}
