using Discord.Commands;

namespace CappuAng.DiscordBot.Models;

public abstract class TextCommand : ModuleBase<SocketCommandContext>
{
    public abstract Task Help();
    public abstract Task Execute();
}
