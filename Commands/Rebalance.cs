using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using OsuSharp.Interfaces;

namespace RebalanceBot.Commands
{
    public class Rebalance : BaseCommandModule
    {
        private IOsuClient Client;

        public Rebalance(IOsuClient client) => Client = client;

        [Command("rebalance"), Aliases("rb"), Description("Check the rebalance for a specific player.")]
        public async Task RebalanceCommand(CommandContext ctx,
            [Description("Username of the player you want to check."), RemainingText]
            string username)
        {
            var user = await Client.GetUserAsync(username);
            
            if (user is null)
            {
                await ctx.Message.RespondAsync($"No user with name {username} found.");
                return;
            }

            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("♨"));
            using (var exeProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments =
                    $"run -- profile {user.Id} {Program.Credentials.OsuApiClientId} {Program.Credentials.OsuApiClientSecret} -o {user.Id}.txt",
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
            }

            await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromUnicode("♨"));

            await using (var fs = new FileStream($"{user.Id}.txt", FileMode.Open))
            {
                await new DiscordMessageBuilder()
                    .WithReply(ctx.Message.Id, true)
                    .WithFile(fs)
                    .SendAsync(ctx.Channel);
            }

            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("✅"));

            try
            {
                File.Delete($"{user.Id}.txt");
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}