using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace GameBot.Modules
{
    public class Waifu : ModuleBase<SocketCommandContext>
    {
        static Random r = new Random();

        [Command("waifu")]
        public async Task WaifuAsync()
        {
            var files = System.IO.Directory.GetFiles(@"Waifus\");

            await Context.Channel.SendFileAsync(files[r.Next(files.Length)]);
        }

        [Command("waifu")]
        public async Task WaifuAsync(string name)
        {
            List<string> files = new List<string>(System.IO.Directory.GetFiles(@"Waifus\", "*.png"));
            files.AddRange(new List<string>(System.IO.Directory.GetFiles(@"Waifus\", "*.gif")));
            files.AddRange(new List<string>(System.IO.Directory.GetFiles(@"Waifus\", "*.jpg")));
            files.Sort();

            //ugly, but hopefully compares the two strings ignoring case, while also omitting the file path from the search
            await Context.Channel.SendFileAsync(files.FirstOrDefault<string>(s => Path.GetFileNameWithoutExtension(s).ToLower().Contains(name.ToLower())));
        }

        [Command("waifu")]
        public async Task WaifuAsync(string arg, string name)
        {
            if (!System.IO.Directory.Exists(@"Waifus\"))
            {
                Directory.CreateDirectory(@"Waifus\");
            }

            var files = System.IO.Directory.GetFiles(@"Waifus\");

            if (arg == "")
            {
                await Context.Channel.SendFileAsync(files[r.Next(files.Length)]);
            }
            else if (arg == "add")
            {
                var attachments = Context.Message.Attachments.GetEnumerator();
                await ReplyAsync("`Not Implemented yet.`");
            }
            else
            {
                await ReplyAsync("usage: \n```+waifu : retrieves your soulmate, scientifically.\n+waifu [name] : retrieves your soulmate, in a more specific fashion.");
            }
        }

    }
}
