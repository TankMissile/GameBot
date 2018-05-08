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
    public class Waifu : ModuleBase<SocketCommandContext>
    {
        static Random r = new Random();

        [Command("waifu")]
        public async Task WaifuAsync()
        {
            var files = GetFiles(@"Waifus\");

            await Context.Channel.SendFileAsync(files[r.Next(files.Length)]);
        }

        [Command("waifu")]
        public async Task WaifuAsync(string arg, params string[] tags)
        {
            if (!Exists(@"Waifus\"))
            {
                CreateDirectory(@"Waifus\");
            }

            if (arg == "")
            {
                var files = GetFiles(@"Waifus\");
                await Context.Channel.SendFileAsync(files[r.Next(files.Length)]);
            }
            else if (arg == "help")
            {
                await ReplyAsync("usage: \n```+waifu : retrieves your soulmate, scientifically.\n" +
                    "+waifu [name] : retrieves your soulmate, in a more specific fashion." +
                    "+waifu add [tags ...] : Upload a new waifu```");
            }
            else if (arg == "add")
            {
                if (Context.Message.Attachments.Count == 0)
                {
                    await Context.Channel.SendMessageAsync("No file attached! Make sure you upload an image and write the command as the comment for it.");
                }

                foreach(var attachment in Context.Message.Attachments)
                {
                    Regex acceptedTypes = new Regex("\\.(png|gif|jpg)$");
                    if (!acceptedTypes.IsMatch(attachment.Filename))
                    {
                        await Context.Channel.SendMessageAsync("Attachment is not an accepted file type.  Only accepts: .png .gif .jpg");
                        return;
                    }

                    string path = "";
                    foreach(string s in tags)
                    {
                        path += s + "_";
                    }
                    path = path.Substring(0, path.Length - 1);
                    path += attachment.Filename.Substring(attachment.Filename.Length - 4);

                    Console.WriteLine("Received file from " + Context.User + ": " + attachment.Filename);
                    Console.WriteLine("Saving as Waifus\\" + path);
                    Console.WriteLine();
                    new WebClient().DownloadFile(attachment.Url, "Waifus\\" + path);
                    await Context.Channel.SendMessageAsync("Uploaded new waifu: " + path);
                }
            }
            else
            {
                List<Regex> regs = new List<Regex>() { new Regex("waifus\\\\.*" + arg.ToLower() + ".*(?:png|gif|jpg)") };
                foreach(string s in tags)
                {
                    regs.Add(new Regex("waifus\\\\.*" + s.ToLower() + ".*(?:png|gif|jpg)"));
                }
                var files = GetFiles(@"Waifus\", "*").ToList();
                files = files.Where(path => {
                    string lowPath = path.ToLower();
                    foreach (Regex reg in regs) {
                        if (!reg.IsMatch(lowPath)) return false;
                    }
                    return true;
                }).ToList();

                if (files.Count() > 0) await Context.Channel.SendFileAsync(files[r.Next(files.Count())]);
                else await Context.Channel.SendMessageAsync("No waifu found :(");
            }
        }
    }
}
