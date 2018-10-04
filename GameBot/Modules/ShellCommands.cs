using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace GameBot.Modules
{
    public class ShellCommands : ModuleBase<SocketCommandContext>
    {
        [Group("minecraft"), Alias("mc")]
        public class Minecraft : ModuleBase<SocketCommandContext>
        {
            [Command("on")]
            public async Task ServerOnAsync()
            {
                System.Diagnostics.Process.Start("CMD.exe", "/C java.exe -jar \"G:\\Minecraft Server 1.13 Vanilla\\server.jar\"");
                Console.Write("Starting minecraft Server");
                await ReplyAsync("Starting minecraft server...");
            }
        }
    }
}
