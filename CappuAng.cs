using System.Text.Json;
using CappuAng.DiscordBot.Controllers;
using CappuAng.DiscordBot.Models;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace CappuAng.DiscordBot;

public class CappuAng
{
	private static readonly DiscordSocketConfig _discordSocketConfig =
		new()
		{
			GatewayIntents =
				(GatewayIntents.AllUnprivileged & ~GatewayIntents.GuildScheduledEvents & ~GatewayIntents.GuildInvites)
				| GatewayIntents.MessageContent
		};

	private static readonly IServiceProvider _serviceProvider = new ServiceCollection()
		.AddSingleton(GetConfiguration())
		.AddSingleton(_discordSocketConfig)
		.AddSingleton(new DiscordSocketClient(_discordSocketConfig))
		.AddSingleton<Logger>()
		.AddSingleton<CommandService>()
		.AddSingleton<TextCommandController>()
		.AddSingleton<InteractionService>()
		.AddSingleton<SlashCommandController>()
		.BuildServiceProvider();

	private static async Task Main()
	{
		DiscordSocketClient discordSocketClient = _serviceProvider.GetRequiredService<DiscordSocketClient>();
		Configuration configuration = _serviceProvider.GetRequiredService<Configuration>();
		await _serviceProvider.GetRequiredService<TextCommandController>().Initialize();
		await _serviceProvider.GetRequiredService<SlashCommandController>().Initialize();

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
				new Log { DateTime = DateTime.Now, Message = exception.Message, Level = LogLevel.Error }
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
