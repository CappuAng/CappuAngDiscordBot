using CappuAngDiscordBot.Models;
using Discord.Commands;

namespace CappuAngDiscordBot.Commands.Text;

[Group("information")]
[Alias("info")]
public class Information : TextCommand
{
    [Command("help")]
    [Alias("h")]
    public override Task Help() => this.ReplyAsync("This is the help message for the Information command.");

    [Command]
    public override Task Execute() => this.ReplyAsync("This is the execution message for the Information command.");
}
