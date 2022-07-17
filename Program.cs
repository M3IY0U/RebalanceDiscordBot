using System;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OsuSharp;
using OsuSharp.Extensions;

namespace RebalanceBot
{
    class Program
    {
        private static DiscordClient _discordClient;
        public static Utilities.Credentials Credentials;
        private static CommandsNextExtension _cnext;

        private static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(
                    "Usage: RebalanceBot.exe <discord token> <osu api client id> <osu api client secret> [bot prefix]");
                return;
            }

            Credentials = new Utilities.Credentials(args[0], Convert.ToInt64(args[1]), args[2]);

            _discordClient = new DiscordClient(new DiscordConfiguration
            {
                Token = Credentials.BotToken,
                MinimumLogLevel = LogLevel.Information
            });


            var serviceProvider = new ServiceCollection()
                .AddOsuSharp(x => x.Configuration = new OsuClientConfiguration
                {
                    ClientId = Credentials.OsuApiClientId,
                    ClientSecret = Credentials.OsuApiClientSecret
                })
                .AddLogging()
                .BuildServiceProvider();

            _cnext = _discordClient.UseCommandsNext(new CommandsNextConfiguration
            {
                EnableDms = false,
                Services = serviceProvider,
                StringPrefixes = new[] { args.Length == 4 ? args[3] : "r!" }
            });

            _cnext.CommandErrored += async (_, eventArgs) =>
            {
                if (eventArgs.Exception.Message.Contains("command was not found"))
                    return;
                await eventArgs.Context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("❎"));
                await eventArgs.Context.RespondAsync($"{eventArgs.Exception.Message}");
            };

            _cnext.RegisterCommands(Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Error registering commands"));
            await _discordClient.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}