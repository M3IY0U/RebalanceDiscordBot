# RebalanceBot
#### Quick Disclaimer this is by no means a bot meant for huge servers or heavy workload (it might work but i can't guarantee anything)
## Requirements
+ Have Dotnet Runtime installed (in order for Performance Calculator to run)
+ Have a Discord Bot + osu!api v1 Key
+ Have the correct folder structure

## Setup Instructions

Make sure that you have the CLI git client installed https://git-scm.com/
### Instructions for getting rework running in general (copied from somewhere idfk where now)
1. Open Git Bash
2. Enter the following commands\
    `git clone https://github.com/ppy/osu-tools`\
    `git clone [the github link of whatever rework you want to check out, ie: https://github.com/emu1337/osu]`
3. If the rework you want is under a branch, you will also need to enter these commands command:
    `cd osu`\
    `git pull`\
    `git checkout [name of branch, ie: ppv2-xexxar-round3]`
4. To update, type and run the following:\
    `cd osu`\
    `git pull`
5. Open the osu-tools folder on your computer (it will probably be located in the Users/user/ directory)
6. Run the file UseLocalOsu.sh / useLocalOsu.ps1
7. Open the PerformanceCalculator folder in the osu-tools folder
8. Open the file titled ProcessWorkingBeatmap.cs in Notepad
9. Near the bottom, you will see a line of code:
    `public override Stream GetStream(string storagePath) => null;`
    all you have to do is put a `//` before the line, like such:
    `//public override Stream GetStream(string storagePath) => null;`
### Instructions for setting up the bot
#### Important: Your project structure _must_ look like this:
```
somefolder/
    ├── osu/
    └── osu-tools/
        └── PerformanceCalculator <-- This is the folder the bot runs in
```
1. Extract the zip archive downloaded from [Releases](https://github.com/M3IY0U/RebalanceDiscordBot/releases) into the PerformanceCalculator folder
2. This is a CLI App, so start a terminal of your choice, navigate to the folder and run it via `RebalanceBot.exe <your discord bot token> <your osu api key> [bot prefix]`
3. Should be running now, if you didn't configure it, the prefix will be `r!` or mentioning the bot

### Available commands (for now): 
+ `r!rebalance <username>` Checks the rebalance for a specific player
+ `r!map <map id>` Recalculates star rating for a specific map
+ `r!update` Updates the osu repo in case any new changes got pushed (This is a bot owner only command)
+ `r!simulate <map id> [accuracy] [mods]` Simulates a play on the given map with provided acc/mods (By default 100% and NM)