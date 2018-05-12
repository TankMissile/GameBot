using Discord.Commands;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GameBot.Modules
{
    /// <summary> Contains various RNG decision makers (coin flip, die roll, 8ball) </summary>
    [Name("RNG")]
    [Summary("Provides commands that return a *scientifically*-chosen result")]
    public class RNG : ModuleBase<SocketCommandContext>
    {
        public static Random random = new Random();

        [Command("flip")]
        [Summary("Flips a coin, resulting in Heads or Tails.")]
        public async Task FlipAsync([Remainder] string filler = null)
        {
            await ReplyAsync("The Coin Flip of Destiny has chosen " + (random.Next(2) == 1 ? "Heads" : "Tails") + "!");
        }

        [Command("roll")]
        [Summary("Roll a variable die, returning a number between 1 and max (default 100).")]
        public async Task RollAsync(int max = 100)
        {
            int roll = random.Next(max) + 1;

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

        [Group("food")]
        [Summary("Provides commands for selecting a place or thing to eat")]
        public class Food : ModuleBase<SocketCommandContext>
        {
            [Command]
            public async Task FoodAsync()
            {
                await ReplyAsync("You should get " + GetRandomLineInFile(@"food.txt") + " today.");
            }

            [Command("add")]
            [Summary("Select a place or thing to eat, using the precise scientific method of choosing randomly.")]
            public async Task AddFoodAsync([Remainder] string text = "")
            {
                if (text != "")
                {
                    AppendStringToFile(@"food.txt", text);
                    await ReplyAsync("Added " + text + " to food list.");
                }
                else
                {
                    await ReplyAsync("usage: `+food add [place or item]`");
                }
            }

            [Command("list")]
            [Summary("Lists all options for food")]
            public async Task ListFoodAsync()
            {
                await ReplyAsync("```" + File.ReadAllText(@"food.txt") + "```");
            }

            [Command("help")]
            [Summary("Provides help text for how to use the `food` command group.")]
            public async Task FoodHelpAsync() {
                await ReplyAsync("Usage:\n```" +
                    "  +food : tells you where/what you should eat.\n" +
                    "  +food add [place/item] : adds an entry to the food list.  Multiple words and punctuation are okay.```");
            }
        }

        [Group("joke")]
        public class Joke : ModuleBase<SocketCommandContext>
        {
            [Command]
            [Summary("Tell an inside joke that normal people will need explained.")]
            public async Task JokeAsync()
            {
                await ReplyAsync(GetRandomLineInFile(@"jokes.txt"), true);
            }

            [Command("add")]
            [Summary("Add a joke to the list")]
            public async Task AddJokeAsync([Remainder] string joke)
            {
                if (joke != "")
                {
                    AppendStringToFile(@"jokes.txt", joke);
                    await ReplyAsync("Added joke to list with ID " + (File.ReadLines(@"jokes.txt").Count()));
                }
                else
                {
                    await ReplyAsync("You didn't even make a joke! Boooooo!");
                }
            }

            [Command]
            [Summary("Retrieve a specific joke, at the given index")]
            public async Task GetJokeAsync(int i)
            {
                await ReplyAsync(GetLineInFile(@"jokes.txt", i - 1), true);
            }

            [Command("list")]
            [Summary("Sends a list of all available jokes, as a private message to the requester")]
            public async Task ListJokesAsync()
            {
                var dm = await Context.User.GetOrCreateDMChannelAsync();

                string[] list = File.ReadAllLines(@"jokes.txt");

                string msg = "";
                for (int i = 0; i < list.Length; i++)
                {
                    msg += (i + 1) + ". " + list[i] + "\n";
                    if ((i + 1) % 30 == 0)
                    {
                        await dm.SendMessageAsync("```" + msg + "```");
                        await Task.Delay(500);
                        msg = "";
                    }
                }
            }

            [Command("help")]
            [Summary("Provides help text on how to use the `joke` commands")]
            public async Task JokeHelpAsync()
            {
                await ReplyAsync("usage:\n```" +
                    "  +joke [id] : Tells a specific joke if ID is provided or a random one if left blank.\n" +
                    "  +joke add [joke] : Adds [joke] to the joke list.  Multiple words and punctuation are okay.```");
            }
        }

        protected static string GetRandomLineInFile(string path)
        {
            var options = File.ReadAllLines(path);
            string msg = options[random.Next(options.Length)];

            return msg;
        }

        protected static string GetLineInFile(string path, int i)
        {
            var options = File.ReadAllLines(path);
            string msg = options[i];

            return msg;
        }

        protected static void AppendStringToFile(string path, string msg, bool isNewLine = true)
        {
            var writer = File.AppendText(path);
            writer.Write((isNewLine ? "\n" : "") + msg);
            writer.Flush();
            writer.Close();
        }
    }
}
