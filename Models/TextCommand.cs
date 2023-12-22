using Discord.Commands;

namespace CappuAngDiscordBot.Models;

public abstract class TextCommand : ModuleBase<SocketCommandContext>
{
    public abstract Task Help();
    public abstract Task Execute();
}
