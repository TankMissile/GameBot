using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Modules
{
    [Summary("Provides commands that involve the squad as a whole")]
    public class Squad : ModuleBase<SocketCommandContext>
    {
        [Command("squad")]
        [Alias("assemble")]
        [Summary("Assemble the squad!")]
        public async Task SquadAsync([Remainder] string arg = "")
        {
            if (Context.User.Username.ToLower().Contains("pham") || Context.User.Username.ToLower().Contains("kaito") || Context.User.Username.ToLower().Contains("hyodo"))
            {
                await ReplyAsync("You have forfeit your summon privileges.");
                return;
            }

            if(arg == "")
            {
                await ReplyAsync("SQUAD, ASSEMBLE!!!", true);
            }
            else
            {
                await ReplyAsync(arg.ToUpper() + ", ASSEMBLE!!!", true);
            }
        }

        [Command("skuado")]
        [Alias("assembru")]
        [Summary("Assemble the skuado!")]
        public async Task SkuadoAsync([Remainder] string arg = "")
        {
            if (Context.User.Username.Contains("apham"))
            {
                await ReplyAsync("You have forfeit your summon privileges.");
                return;
            }

            if (arg == "")
            {
                await ReplyAsync("SKUADO, ASSEMBRUUUUU!!!", true);
            }
            else
            {
                await ReplyAsync(arg.ToUpper() + ", ASSEMBRUUUUU!!!", true);
            }
        }

        [Command("duaqs")]
        [Alias("elbmessa")]
        [Summary(".duaqs eht elbmessA")]
        public async Task DuaqsAsync([Remainder] string arg = "")
        {
            await ReplyAsync("!elbmessa ,duaqs", true);
        }

        [Command("disperse")]
        [Summary("Disperse the squad!")]
        public async Task DisperseAsync([Remainder] string arg = "")
        {
            await ReplyAsync("SQUAD, DISPERSE!!!", true);
        }
    }
}
