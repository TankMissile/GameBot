﻿using Discord.Commands;
using System.Threading.Tasks;

namespace GameBot.Modules
{
    [Summary("Provides commands used to alter the very threads of life and death")]
    public class Raise : ModuleBase<SocketCommandContext>
    {
        [Command("raise")]
        public async Task RaiseAsync(string name = "")
        {
            if (name.ToLower() == "gamebot" || name == "")
            {
                await ReplyAsync("I LIVE!!!!", GameBot.TryUseTts(Context.User.Id));
            }
            else
            {
                await ReplyAsync("COME BACK TO US, " + name.ToUpper() + "!!!", GameBot.TryUseTts(Context.User.Id));
            }
        }

        [Command("suicide")]
        [Alias("dead")]
        public async Task SuicideAsync()
        {
            await ReplyAsync("```  ═╦═══╗\n" +
                                "   Ó   ║\n" +
                                "  /|\\  ║\n" +
                                "  / \\  ║\n" +
                                " ══════╩══```");
        }
    }
}
