using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Modules
{
    public class Squad : ModuleBase<SocketCommandContext>
    {
        [Command("squad")]
        [Alias("assemble")]
        [Summary("Assemble the squad!")]
        public async Task SquadAsync([Remainder] string arg = "")
        {
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
            if (arg == "")
            {
                await ReplyAsync("SKUADO, ASSEMBRUUUUU!!!", true);
            }
            else
            {
                await ReplyAsync(arg.ToUpper() + ", ASSEMBRUUUUU!!!");
            }
        }

        [Command("disperse")]
        [Summary("Disperse the squad!")]
        public async Task DisperseAsync([Remainder] string arg = "")
        {
            await ReplyAsync("SQUAD, DISPERSE!!!", true);
        }
    }
}
