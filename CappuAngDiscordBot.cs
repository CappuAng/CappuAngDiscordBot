using System.Text.Json;
using CappuAngDiscordBot.Controllers;
using CappuAngDiscordBot.Models;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace CappuAngDiscordBot;

public class CappuAngDiscordBot
{
	private static readonly DiscordSocketConfig discordSocketConfig =
		new()
		{
			GatewayIntents =
				(GatewayIntents.AllUnprivileged & ~GatewayIntents.GuildScheduledEvents & ~GatewayIntents.GuildInvites)
				| GatewayIntents.MessageContent
		};

	private static readonly IServiceProvider serviceProvider = new ServiceCollection()
		.AddSingleton(CappuAngDiscordBot.GetConfiguration())
		.AddSingleton(CappuAngDiscordBot.discordSocketConfig)
		.AddSingleton(new DiscordSocketClient(CappuAngDiscordBot.discordSocketConfig))
		.AddSingleton<Logger>()
		.AddSingleton<CommandService>()
		.AddSingleton<TextCommandController>()
		.AddSingleton<InteractionService>()
		.AddSingleton<SlashCommandController>()
		.BuildServiceProvider();

	private static async Task Main()
	{
		DiscordSocketClient discordSocketClient = CappuAngDiscordBot.serviceProvider.GetRequiredService<DiscordSocketClient>();
		Configuration configuration = CappuAngDiscordBot.serviceProvider.GetRequiredService<Configuration>();
		await CappuAngDiscordBot.serviceProvider.GetRequiredService<TextCommandController>().Initialize();
		await CappuAngDiscordBot.serviceProvider.GetRequiredService<SlashCommandController>().Initialize();

		discordSocketClient.Log += Logger.Log;

		await discordSocketClient.LoginAsync(TokenType.Bot, configuration.Token);
		await discordSocketClient.StartAsync();
		await Task.Delay(Timeout.Infinite);
	}

	private static Configuration GetConfiguration()
	{
		try
		{
			return JsonSerializer.Deserialize<Configuration>(File.ReadAllText("Configuration.json"))
				?? throw new ArgumentNullException();
		}
		catch (Exception exception)
		{
			_ = Logger.Log(
				new Log
				{
					DateTime = DateTime.Now,
					Message = exception.Message,
					Level = LogLevel.Error
				}
			);

			Environment.Exit(1);
			throw;
		}
	}

	public static bool IsDebug() =>
#if DEBUG
		true;
#else
		false;
#endif
}
