using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Modules
{
    public class Raise : ModuleBase<SocketCommandContext>
    {
        [Command("raise")]
        public async Task RaiseAsync(string name = "")
        {
            if (name.ToLower() == "gamebot" || name == "")
            {
                await ReplyAsync("***I LIVE!!!!***", true);
            }
            else
            {
                await ReplyAsync("*BREATHE, " + name.ToUpper() + ",* ***BREATHE!***", true);
            }
        }

    }
}
