using System.Diagnostics;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace RebalanceBot.Commands
{
    public class Update : BaseCommandModule
    {
        [Command("update"), RequireOwner]
        public async Task UpdateCommand(CommandContext ctx)
        {
            using (var exeProcess = Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = "../../osu",
                FileName = "git",
                Arguments = "pull",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }))
            {
                if (exeProcess is null)
                {
                    await ctx.Message.RespondAsync("Error spawning update process.");
                    return;
                }

                await exeProcess.WaitForExitAsync();
                var output = await exeProcess.StandardOutput.ReadToEndAsync();
                var error = await exeProcess.StandardError.ReadToEndAsync();

                if (!string.IsNullOrEmpty(output))
                    await ctx.Message.RespondAsync($"Output:```\n{output}\n```");
                if (!string.IsNullOrEmpty(error))
                    await ctx.Message.RespondAsync($"Error:```\n{error}\n```");
            }
        }
    }
}