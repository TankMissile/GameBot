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
        static Random r = new Random();

        [Command]
        public async Task WaifuAsync()
        {
            if (!Exists(@"Waifus\"))
            {
                await ReplyAsync("No waifus yet :(");
                await ReplyAsync("Add some with `+waifu add`!");
                return;
            }

            var files = GetFiles(@"Waifus\");

            await Context.Channel.SendFileAsync(files[r.Next(files.Length)]);
        }

        [Command]
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
            else if (arg == "help" || arg == "?")
            {
                await ReplyAsync("usage: \n```+waifu : retrieves your soulmate, scientifically.\n" +
                    "+waifu [name] : retrieves your soulmate, in a more specific fashion.\n" +
                    "+waifu add [tags ...] : Upload a new waifu```");
            }
            else if (arg == "add")
            {
                if (Context.Message.Attachments.Count == 0)
                {
                    await ReplyAsync("No file attached! Make sure you upload an image and write the command as the comment for it.");
                    return;
                }

                if(tags.Length == 0)
                {
                    await ReplyAsync("You need to add tags!  usage: `+waifu add [tags ...]`");
                    return;
                }

                foreach(var attachment in Context.Message.Attachments)
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
                    foreach(string s in tags)
                    {
                        salted = accepted.Replace(s, ""); //not a fan of the way this is set up
                        if (salted.Length == 0) continue;

                        path += salted + "_";
                    }
                    path = path.Substring(0, path.Length - 1);
                    path += attachment.Filename.Substring(attachment.Filename.Length - 4);

                    Console.WriteLine("Received file from {0}: {1}", Context.User, attachment.Filename);
                    Console.WriteLine("Saving as Waifus\\{0}", path);
                    Console.WriteLine();
                    new WebClient().DownloadFile(attachment.Url, "Waifus\\" + path);
                    await ReplyAsync("Uploaded new waifu: " + path);
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
                else await ReplyAsync("No waifu found :(");
            }
        }
    }
}
