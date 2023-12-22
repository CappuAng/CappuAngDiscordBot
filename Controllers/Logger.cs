using CappuAngDiscordBot.Models;
using Discord;

namespace CappuAngDiscordBot.Controllers;

public class Logger
{
    public static Task Log(Log log)
    {
        Console.ForegroundColor = log.Level switch
        {
            LogLevel.Information => ConsoleColor.Blue,
            LogLevel.Success => ConsoleColor.Green,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            _ => ConsoleColor.Blue
        };
        Console.WriteLine($"[{log.DateTime:yyyy-MM-dd HH:mm:ss}] [{log.Level}] {log.Message}");
        Console.ResetColor();
        return Task.CompletedTask;
    }

    public static Task Log(string message, LogLevel logLevel) =>
        Logger.Log(
            new Log
            {
                DateTime = DateTime.Now,
                Message = message,
                Level = logLevel
            }
        );

    public static Task Log(LogMessage logMessage) =>
        Logger.Log(
            new Log
            {
                DateTime = DateTime.Now,
                Message = logMessage.Message,
                Level = logMessage.Severity switch
                {
                    LogSeverity.Critical => LogLevel.Error,
                    LogSeverity.Error => LogLevel.Error,
                    LogSeverity.Warning => LogLevel.Warning,
                    LogSeverity.Info => LogLevel.Information,
                    LogSeverity.Verbose => LogLevel.Information,
                    LogSeverity.Debug => LogLevel.Information,
                    _ => LogLevel.Information
                }
            }
        );
}
