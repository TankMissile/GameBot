using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Modules
{
    public class Text : ModuleBase<SocketCommandContext>
    {
        [Command("raise")]
        public async Task RaiseAsync(string name = "")
        {
            if (name.ToLower() == "gamebot" || name == "")
            {
                await ReplyAsync("I LIVE!!!!", true);
            }
            else
            {
                await ReplyAsync("COME BACK TO US, " + name.ToUpper() + "!!!", true);
            }
        }

        [Command("Suicide")]
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
