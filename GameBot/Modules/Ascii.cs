using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace GameBot.Modules
{
    public class Ascii : ModuleBase<SocketCommandContext>
    {
        [Command("lenny")]
        [Summary("Produces a Lenny face")]
        public async Task LennyAsync()
        {
            await ReplyAsync("( ͡° ͜ʖ ͡°)");
        }

        [Command("tableflip")]
        [Summary("Flips the table in rage")]
        public async Task TableflipAsync()
        {
            await ReplyAsync("(╯°□°）╯︵ ┻━┻");
        }

        [Command("tablefix")]
        [Summary("Puts the table back in place")]
        public async Task TablefixAsync()
        {
            await ReplyAsync("┬─┬ノ( ° _ °ノ)");
        }

        [Command("donger")]
        [Summary("Raises dongers")]
        public async Task DongerAsync()
        {
            await ReplyAsync("ヽ༼ຈل͜ຈ༽ﾉ");
        }

        [Command("pagandance"), Alias("pagan")]
        [Summary("Performs a pagan ritual")]
        public async Task PaganDanceAsync()
        {
            await ReplyAsync("ヽ(´ー｀)ﾉヽ(´ー｀)ﾉヽ(´ー｀)ﾉ");
        }
    }
}
