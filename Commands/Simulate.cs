using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using OsuSharp.Interfaces;

namespace RebalanceBot.Commands
{
    public class Simulate : BaseCommandModule
    {
        private IOsuClient Client;

        public Simulate(IOsuClient client) => Client = client;

        [Command("simulate"), Aliases("sim", "s"), Description("Simulate a play on a specific map.")]
        public async Task SimCommand(CommandContext ctx,
            [Description("ID of the map you want to simulate")]
            string mapId,
            [Description("Accuracy of the play you want to simulate")]
            double acc = 100,
            [Description("Mods of the play you want to simulate")]
            string mods = "")
        {
            var map = await Client.GetBeatmapAsync(Convert.ToInt64(mapId));

            if (!File.Exists($"cache/{mapId}.osu"))
            {
                if (map is null)
                {
                    await ctx.Message.RespondAsync($"No map with id {mapId} was found.");
                    return;
                }

                await Utilities.DownloadMap(mapId);
            }

            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("♨"));
            using (var exeProcess = Process.Start(new ProcessStartInfo
                   {
                       FileName = "dotnet",
                       Arguments =
                           $"run -- simulate osu cache/{mapId}.osu -a {acc} {ParseMods(mods)} -o {mapId}.txt",
                       UseShellExecute = false,
                       WindowStyle = ProcessWindowStyle.Hidden,
                       CreateNoWindow = true
                   }))
            {
                if (exeProcess is null)
                {
                    await ctx.Message.RespondAsync("Error spawning process.");
                    return;
                }

                await exeProcess.WaitForExitAsync();

                await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromUnicode("♨"));

                await using (var fs = new FileStream($"{mapId}.txt", FileMode.Open))
                {
                    await new DiscordMessageBuilder()
                        .WithReply(ctx.Message.Id, true)
                        .WithFile(fs)
                        .SendAsync(ctx.Channel);
                }
            }

            try
            {
                File.Delete($"{mapId}.txt");
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static string ParseMods(string unformatted)
        {
            var rx = new Regex("hr|hd|dt|nc|ez|nf|ht|fl");
            var matches = rx.Matches(unformatted).Select(m => "-m " + m.Value);
            return string.Join(" ", matches);
        }
    }
}