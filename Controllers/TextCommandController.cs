using System.Reflection;
using CappuAng.DiscordBot.Models;
using Discord.Commands;
using Discord.WebSocket;

namespace CappuAng.DiscordBot.Controllers;

public class TextCommandController(
    DiscordSocketClient discordSocketClient,
    CommandService commandService,
    Configuration configuration,
    IServiceProvider serviceProvider
)
{
    private readonly DiscordSocketClient discordSocketClient = discordSocketClient;
    private readonly CommandService commandService = commandService;
    private readonly Configuration configuration = configuration;
    private readonly IServiceProvider serviceProvider = serviceProvider;

    private static readonly Type[] _textCommands = Assembly
        .GetExecutingAssembly()
        .GetTypes()
        .Where(
            (type) =>
                type.IsSubclassOf(typeof(TextCommand)) && type.Namespace == "CappuCappsDiscordBot.Commands.Text"
        )
        .ToArray();

    public Task Initialize()
    {
        discordSocketClient.Ready += RegisterTextCommands;
        discordSocketClient.MessageReceived += HandleTextCommand;
        return Task.CompletedTask;
    }

    private async Task RegisterTextCommands()
    {
        try
        {
            await Logger.Log("Registering text commands...", LogLevel.Information);

            foreach (Type textCommand in _textCommands)
            {
                await Logger.Log($"Registering text command: {textCommand.Name}...", LogLevel.Information);
                ModuleInfo moduleInfo = await commandService.AddModuleAsync(textCommand, serviceProvider);
                await Logger.Log($"Registered text command: {moduleInfo.Name}", LogLevel.Success);
            }

            await Logger.Log("Registered all available text commands.", LogLevel.Success);
        }
        catch (Exception exception)
        {
            await Logger.Log(exception.Message, LogLevel.Error);
        }
    }

    private async Task HandleTextCommand(SocketMessage socketMessage)
    {
        try
        {
            // Check if is a socket user message and store it to a variable
            if (socketMessage is not SocketUserMessage socketUserMessage)
                return;

            // Check if the autor is not a bot
            if (socketUserMessage.Author.IsBot)
                return;

            // Check if the message has the correct prefix or mention
            bool hasPrefix = false;
            int argumentPosition = 0;

            foreach (string prefix in configuration.Prefixes)
            {
                if (socketUserMessage.HasStringPrefix(prefix, ref argumentPosition))
                {
                    hasPrefix = true;
                    break;
                }
            }

            if (
                !hasPrefix
                && socketUserMessage.HasMentionPrefix(discordSocketClient.CurrentUser, ref argumentPosition)
            )
                hasPrefix = true;

            if (!hasPrefix)
                return;

            // Execute the command
            SocketCommandContext socketCommandContext = new(discordSocketClient, socketUserMessage);
            _ = await commandService.ExecuteAsync(socketCommandContext, argumentPosition, serviceProvider);
        }
        catch (Exception exception)
        {
            await Logger.Log(exception.Message, LogLevel.Error);
        }
    }
}
