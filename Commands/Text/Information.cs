using CappuAng.DiscordBot.Models;
using Discord.Commands;

namespace CappuAng.DiscordBot.Commands.Text;

[Group("information")]
[Alias("info")]
public class Information : TextCommand
{
    [Command("help")]
    [Alias("h")]
    public override Task Help() => ReplyAsync("This is the help message for the Information command.");

    [Command]
    public override Task Execute() => ReplyAsync("This is the execution message for the Information command.");
}
