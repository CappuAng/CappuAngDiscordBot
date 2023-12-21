using Discord.Interactions;

namespace CappuAng.DiscordBot.Models;

public abstract class SlashCommand : InteractionModuleBase<SocketInteractionContext>
{
    public abstract Task Execute();
}