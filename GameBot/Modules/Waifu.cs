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
    [Name("Waifu")]
    [Summary("Provides commands involving trashy waifus")]
    public class Waifu : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private static readonly string PATH = GameBot.GetPath(@"Waifus\");

        private static string[] lastTags = null;

        public Waifu(IServiceProvider prov, CommandService comm)
        {
            _commands = comm;
            _services = prov;
        }

        [Command, Name("waifu")]
        [Summary("Retrieves your soulmate, scientifically.")]
        public async Task WaifuAsync()
        {
            var files = GetFiles(PATH);
            if(files.Length == 0)
            {
                await ReplyAsync(RNG.Error.GetRandomErrorMessage("There are no waifus!"));
                return;
            }
            
            var file = files[RNG.random.Next(files.Count())];
            Console.WriteLine($"Sending {file} to channel {Context.Channel.Name}");
            var reg = new Regex(@"[\\/]([\w_ ]+).(?:png|gif|jpeg|jpeg)");
            lastTags = reg.Match(file).Groups[1].Value.Split("_");
            await Context.Channel.SendFileAsync(file);
        }

        [Command, Name("waifu")]
        [Priority(-1)]
        [Summary("Searches the waifu pool for waifus containing all provided tags (as whole words).  If there are multiple, chooses one scientifically.")]
        public async Task WaifuAsync(params string[] tags)
        {
            List<Regex> regs = new List<Regex>();
            foreach (string s in tags)
            {
                regs.Add(new Regex(@"waifus.*[\\/_ ]" + s.ToLower() + @"[ _.].*(?:png|gif|jpg)"));
            }
            var files = GetFiles(PATH, "*").ToList();
            files = files.Where(path => {
                string lowPath = path.ToLower();
                foreach (Regex reg in regs)
                {
                    if (!reg.IsMatch(lowPath)) return false;
                }
                return true;
            }).ToList();

            if (files.Count() > 0)
            {
                var file = files[RNG.random.Next(files.Count())];
                Console.WriteLine("Sending {0} to channel {1}", file, Context.Channel.Name);
                await Context.Channel.SendFileAsync(file);
            }
            else await ReplyAsync("No waifu found :(");
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
                Console.WriteLine("Saving as {0}{1}", PATH, path);
                new WebClient().DownloadFile(attachment.Url, PATH + path);
                await ReplyAsync("Uploaded new waifu: " + path);
            }
        }

        [Command("best")]
        [Summary("Scientifically calculates the best waifu of them all")]
        public async Task GetBestWaifuAsync()
        {
            await ReplyAsync("#1 Waifu NA:");
            await WaifuAsync();
        }

        [Command("worst")]
        [Summary("Scientifically calculates the worst waifu of them all")]
        public async Task GetWorstWaifuAsync()
        {
            await ReplyAsync("Trashiest cancer waifu of all time:");
            await WaifuAsync();
        }

        [Command("who"), Alias("which", "last", "dare")]
        [Summary("Reveals the tags of the last randomly-summoned waifu")]
        public async Task LastAsync()
        {
            await ReplyAsync($"Last random waifu had these tags: {string.Join(", ", lastTags.ToArray())}");
        }
    }
}
