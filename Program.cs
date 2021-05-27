using System;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OsuSharp;

namespace RebalanceBot
{
    class Program
    {
        private static DiscordClient _discordClient;
        private static OsuClient _osuClient;
        public static Utilities.Credentials Credentials;
        private static CommandsNextExtension _cnext;

        private static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: RebalanceBot.exe <discord token> <osu api key> [bot prefix]");
                return;
            }

            Credentials = new Utilities.Credentials(args[0], args[1]);

            _discordClient = new DiscordClient(new DiscordConfiguration
            {
                Token = Credentials.BotToken,
                MinimumLogLevel = LogLevel.Information
            });

            _osuClient = new OsuClient(new OsuSharpConfiguration {ApiKey = Credentials.OsuApiKey});

            var serviceProvider = new ServiceCollection()
                .AddSingleton(_osuClient)
                .BuildServiceProvider();

            _cnext = _discordClient.UseCommandsNext(new CommandsNextConfiguration
            {
                EnableDms = false,
                Services = serviceProvider,
                StringPrefixes = new[] {args.Length == 3 ? args[2] : "r!"}
            });

            _cnext.RegisterCommands(Assembly.GetEntryAssembly());
            await _discordClient.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}