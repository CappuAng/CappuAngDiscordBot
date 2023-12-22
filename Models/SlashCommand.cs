using Discord.Interactions;

namespace CappuAngDiscordBot.Models;

public abstract class SlashCommand : InteractionModuleBase<SocketInteractionContext>
{
    public abstract Task Execute();
}