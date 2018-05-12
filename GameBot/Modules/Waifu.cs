using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static System.IO.Directory;
using System.Text.RegularExpressions;
using System.Net;

namespace GameBot.Modules
{
    [Group("waifu")]
    public class Waifu : ModuleBase<SocketCommandContext>
    {
        static Waifu() {
            if (!Exists(@"Waifus\"))
            {
                CreateDirectory(@"Waifus\");
            }
        }

        [Command, Name("Waifu")]
        [Summary("Retrieves your soulmate, scientifically.")]
        public async Task WaifuAsync()
        {
            var files = GetFiles(@"Waifus\");
            var file = files[RNG.random.Next(files.Count())];
            Console.WriteLine("Sending {0} to channel {1}", file, Context.Channel.Name);
            await Context.Channel.SendFileAsync(file);
        }

        [Command("help"), Alias("?")]
        [Summary("Gives some help on command usage")]
        public async Task WaifuHelpAsync()
        {
            await ReplyAsync("usage: \n```+waifu : retrieves your soulmate, scientifically.\n" +
                    "+waifu [name] : retrieves your soulmate, in a more specific fashion.\n" +
                    "+waifu add [tags ...] : Upload a new waifu```");
        }

        [Command("add")]
        [Summary("Adds a new waifu to the pool, with a filename corresponding to the provided tags.")]
        public async Task WaifuAddAsync(params string[] tags)
        {
            if (Context.Message.Attachments.Count == 0)
            {
                await ReplyAsync("No file attached! Make sure you upload an image and write the command as the comment for it.");
                return;
            }

            if (tags.Length == 0)
            {
                await ReplyAsync("You need to add tags!  usage: `+waifu add [tags ...]`");
                return;
            }

            foreach (var attachment in Context.Message.Attachments)
            {
                Regex accepted = new Regex("\\.(png|gif|jpg)$");
                if (!accepted.IsMatch(attachment.Filename))
                {
                    await ReplyAsync("Attachment is not an accepted file type.  Only accepts: .png .gif .jpg");
                    return;
                }

                string path = "";
                string salted;
                accepted = new Regex("[./\\@#$%^&*,<>\\[\\]{}\\(\\)`~;':\"|?]+");
                foreach (string s in tags)
                {
                    salted = accepted.Replace(s, ""); //not a fan of the way this is set up
                    if (salted.Length == 0) continue;

                    path += salted + "_";
                }
                path = path.Substring(0, path.Length - 1);
                path += attachment.Filename.Substring(attachment.Filename.Length - 4);

                Console.WriteLine("\nReceived file from {0}: {1}", Context.User, attachment.Filename);
                Console.WriteLine("Saving as Waifus\\{0}", path);
                new WebClient().DownloadFile(attachment.Url, "Waifus\\" + path);
                await ReplyAsync("Uploaded new waifu: " + path);
            }
        }

        [Command, Priority(-1)]
        [Summary("Searches the waifu pool for waifus containing all provided tags (as whole words).  If there are multiple, chooses one scientifically.")]
        public async Task WaifuAsync(params string[] tags)
        {
            List<Regex> regs = new List<Regex>();
            foreach(string s in tags)
            {
                regs.Add(new Regex("waifus.*[\\\\/_]" + s.ToLower() + "[_.].*(?:png|gif|jpg)"));
            }
            var files = GetFiles(@"Waifus\", "*").ToList();
            files = files.Where(path => {
                string lowPath = path.ToLower();
                foreach (Regex reg in regs)
                {
                    Console.WriteLine("Testing {0} against {1}", lowPath, reg.ToString());
                    if (!reg.IsMatch(lowPath)) return false;
                }
                return true;
            }).ToList();

            if (files.Count() > 0)
            {
                var file = files[RNG.random.Next(files.Count())];
                Console.WriteLine("Sending {0} to channel {1}",  file, Context.Channel.Name);
                await Context.Channel.SendFileAsync(file);
            }
            else await ReplyAsync("No waifu found :(");
        }
    }
}
