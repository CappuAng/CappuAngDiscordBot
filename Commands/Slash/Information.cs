using CappuAngDiscordBot.Models;
using Discord.Interactions;

namespace CappuAngDiscordBot.Commands.Slash;

public class Information : SlashCommand
{
    [SlashCommand("information", "Shows information about the bot.")]
    public override async Task Execute() =>
        await this.RespondAsync("This is the execution message for the Information command.");
}
