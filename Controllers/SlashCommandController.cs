using System.Reflection;
using CappuAng.DiscordBot.Models;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;

namespace CappuAng.DiscordBot.Controllers;

public class SlashCommandController(
    DiscordSocketClient discordSocketClient,
    InteractionService interactionService,
    Configuration configuration,
    IServiceProvider serviceProvider
)
{
    private readonly DiscordSocketClient discordSocketClient = discordSocketClient;
    private readonly InteractionService interactionService = interactionService;
    private readonly Configuration configuration = configuration;
    private readonly IServiceProvider serviceProvider = serviceProvider;

    private static readonly Type[] _slashCommands = Assembly
        .GetExecutingAssembly()
        .GetTypes()
        .Where(
            type => type.IsSubclassOf(typeof(SlashCommand)) && type.Namespace == "CappuCappsDiscordBot.Commands.Slash"
        )
        .ToArray();

    public Task Initialize()
    {
        discordSocketClient.Ready += RegisterSlashCommands;
        discordSocketClient.InteractionCreated += HandleSlashCommand;
        return Task.CompletedTask;
    }

    private async Task RegisterSlashCommands()
    {
        try
        {
            await Logger.Log("Registering slash commands locally...", LogLevel.Information);

            foreach (Type slashCommand in _slashCommands)
            {
                await Logger.Log($"Registering slash command locally: {slashCommand.Name}...", LogLevel.Information);

                ModuleInfo moduleInfo = await interactionService.AddModuleAsync(
                    slashCommand,
                    serviceProvider
                );

                await Logger.Log($"Registered slash command locally: {moduleInfo.Name}", LogLevel.Success);
            }

            await Logger.Log("Registered all available slash commands locally.", LogLevel.Success);

            if (CappuAng.IsDebug())
            {
                await Logger.Log("Registering slash commands to the Guild in Discord...", LogLevel.Information);

                IReadOnlyCollection<RestGuildCommand> restGuildCommands =
                    await interactionService.RegisterCommandsToGuildAsync(configuration.GuildId, true);

                foreach (RestGuildCommand restGuildCommand in restGuildCommands)
                {
                    await Logger.Log(
                        $"Registered slash command to the Guild in Discord: {restGuildCommand.Name}",
                        LogLevel.Success
                    );
                }

                await Logger.Log("Registered all slash command to the Guild in Discord.", LogLevel.Success);
            }
            else
            {
                await Logger.Log("Registering slash commands globally in Discord...", LogLevel.Information);

                IReadOnlyCollection<RestGlobalCommand> restGlobalCommands =
                    await interactionService.RegisterCommandsGloballyAsync(true);

                foreach (RestGlobalCommand restGlobalCommand in restGlobalCommands)
                {
                    await Logger.Log(
                        $"Registered slash command globally in Discord: {restGlobalCommand.Name}",
                        LogLevel.Success
                    );
                }

                await Logger.Log("Registered all slash commands globally in Discord.", LogLevel.Success);
            }
        }
        catch (Exception exception)
        {
            await Logger.Log(exception.Message, LogLevel.Error);
        }
    }

    private async Task HandleSlashCommand(SocketInteraction socketInteraction)
    {
        try
        {
            SocketInteractionContext socketInteractionContext = new(discordSocketClient, socketInteraction);
            IResult result = await interactionService.ExecuteCommandAsync(
                socketInteractionContext,
                serviceProvider
            );
        }
        catch (Exception exception)
        {
            await Logger.Log(exception.Message, LogLevel.Error);

            if (socketInteraction.Type is Discord.InteractionType.ApplicationCommand)
            {
                _ = await socketInteraction
                    .GetOriginalResponseAsync()
                    .ContinueWith(
                        async (restInteractionMessage) =>
                            await restInteractionMessage.Result.DeleteAsync()
                    );
            }
        }
    }
}
