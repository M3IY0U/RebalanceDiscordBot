using System;
using System.Collections.Generic;
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
        public static KeyValuePair<string, string> Tokens;
        private static CommandsNextExtension _cnext;

        private static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: RebalanceBot.exe <discord token> <osu api key>");
                return;
            }

            Tokens = new KeyValuePair<string, string>($"{args[0]}", $"{args[1]}");

            _discordClient = new DiscordClient(new DiscordConfiguration
            {
                Token = Tokens.Key,
                MinimumLogLevel = LogLevel.Information
            });

            _osuClient = new OsuClient(new OsuSharpConfiguration {ApiKey = Tokens.Value});

            var serviceProvider = new ServiceCollection()
                .AddSingleton(_osuClient)
                .BuildServiceProvider();

            _cnext = _discordClient.UseCommandsNext(new CommandsNextConfiguration
            {
                EnableDms = false,
                Services = serviceProvider,
                StringPrefixes = new[] {"r!"}
            });

            _cnext.RegisterCommands(Assembly.GetEntryAssembly());
            await _discordClient.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}