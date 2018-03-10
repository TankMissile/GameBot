using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Modules
{
    //Contains various RNG decision makers (coin flip, die roll, 8ball)
    //[Group("rng")]
    public class RNG : ModuleBase<SocketCommandContext>
    {
        Random r = new Random();

        [Command("flip")]
        [Summary("Flips a coin, resulting in Heads or Tails.")]
        public async Task FlipAsync([Remainder] string filler = null)
        {
            await ReplyAsync("The Coin Flip of Destiny has chosen " + (r.Next(2) == 1 ? "Heads" : "Tails") + "!");
        }

        [Command("roll")]
        [Summary("Roll a variable die, returning a number between 1 and max (default 100).")]
        public async Task RollAsync([Summary("Maximum number achievable")] int max = 100)
        {
            int roll = r.Next(max) + 1;

            int firstDigit = roll;
            while (firstDigit >= 10)
            {
                firstDigit /= 10;
            }

            string msg = Context.User.Username + " rolls a" + (roll==11 || roll == 18 || firstDigit == 8 ? "n " : " ") + roll + "!";
            if (roll == 1) msg += " Critical failure!";
            else if (roll == max) msg += "Critical success!";

            await ReplyAsync(msg);
        }

        [Command("8ball")]
        [Summary("Ask a yes or no question to the Sarcastic 8 Ball.")]
        public async Task EightBallAsync([Remainder] string question = null)
        {

            await ReplyAsync("The Magic 8 Ball says: " + GetRandomLineInFile(@"8ball.txt"));
        }

        [Command("food")]
        [Summary("Select a place or thing to eat, using the precise scientific method of choosing randomly.")]
        public async Task FoodAsync(string arg1 = "", [Remainder] string arg2 = "")
        {
            if (arg1 == "")
            {
                await ReplyAsync("You should get " + GetRandomLineInFile(@"food.txt") + " today.");
            }
            else if (arg1.ToLower() == "add")
            {
                if (arg2 != "")
                {
                    AppendStringToFile(@"food.txt", arg2);
                    await ReplyAsync("Added " + arg2 + " to food list.");
                }
                else
                {
                    await ReplyAsync("use: +food add [place or item]");
                }

            }
            else
            {
                await ReplyAsync("Usage:\n```  +food : tells you where/what you should eat.\n+food add [place/item] : adds an entry to the food list.  Multiple words and punctuation are okay.```");
            }
        }

        [Command("joke")]
        [Summary("Tell an inside joke that normal people will need explained.")]
        public async Task JokeAsync(string arg = "", [Remainder] string joke = "")
        {
            if(arg == "")
            {
                await ReplyAsync(GetRandomLineInFile(@"jokes.txt"), true);
            }
            else if (arg.ToLower() == "add" && joke != "")
            {
                AppendStringToFile(@"jokes.txt", joke);
                await ReplyAsync("Added joke to list with ID " + (File.ReadLines(@"jokes.txt").Count()));
            }
            else if (Int32.TryParse(arg, out int i))
            {
                await ReplyAsync(GetLineInFile(@"jokes.txt", i-1), true);
            }
            else if (arg.ToLower() == "list")
            {
                var dm = await Context.User.GetOrCreateDMChannelAsync();

                string[] list = File.ReadAllLines(@"jokes.txt");

                string msg = "";
                for (i = 0; i < list.Length; i++)
                {
                    msg += (i + 1) + ". " + list[i] + "\n";
                    if ((i+1) % 30 == 0)
                    {
                        await dm.SendMessageAsync("```" + msg + "```");
                        await Task.Delay(500);
                        msg = "";
                    }
                }
            }
            else
            {
                await ReplyAsync("usage:\n```  +joke [id] : Tells a specific joke if ID is provided or a random one if left blank.\n  +joke add [joke] : Adds [joke] to the joke list.  Multiple words and punctuation are okay.```");
            }
        }

        private string GetRandomLineInFile(string path)
        {
            var options = File.ReadAllLines(path);
            string msg = options[r.Next(options.Length)];

            return msg;
        }

        private string GetLineInFile(string path, int i)
        {
            var options = File.ReadAllLines(path);
            string msg = options[i];

            return msg;
        }

        private void AppendStringToFile(string path, string msg, bool isNewLine = true)
        {
            var writer = File.AppendText(path);
            writer.Write((isNewLine ? "\n" : "") + msg);
            writer.Flush();
            writer.Close();
        }
    }
}
