using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using OsuSharp;

namespace RebalanceBot.Commands
{
    public class Map : BaseCommandModule
    {
        private OsuClient Client;

        public Map(OsuClient client) => Client = client;

        [Command("map"), Description("Recalculate star rating for a specific map.")]
        public async Task MapCommand(CommandContext ctx, [Description("ID of the map you want to recalc")]string mapId)
        {
            var map = await Client.GetBeatmapByIdAsync(Convert.ToInt64(mapId), GameMode.Standard);

            if (!File.Exists($"cache/{mapId}.osu"))
            {
                if (map is null)
                {
                    await ctx.Message.RespondAsync($"No map with id {mapId} was found.");
                    return;
                }

                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("📥"));
                await Utilities.DownloadMap(mapId);
                await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromUnicode("📥"));
            }

            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("♨"));
            using (var exeProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments =
                    $"run -- difficulty cache/{mapId}.osu -o {mapId}.txt",
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
            }))
            {
                if (exeProcess is null)
                {
                    await ctx.Message.RespondAsync("Error spawning process.");
                    return;
                }

                await exeProcess.WaitForExitAsync();

                await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromUnicode("♨"));

                using (var fs = new FileStream($"{mapId}.txt", FileMode.Open))
                {
                    await new DiscordMessageBuilder()
                        .WithContent($"Old Star Rating: {Math.Round(map.StarRating.GetValueOrDefault(), 2)}")
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
    }
}