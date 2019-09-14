using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

using GameBot.Modules.DB;

namespace GameBot.Modules
{
    /// <summary> Contains various RNG decision makers (coin flip, die roll, 8ball, jokes, food, etc) </summary>
    [Name("RNG")]
    [Summary("Provides commands that return a *scientifically*-chosen result")]
    public class RNG : ModuleBase<SocketCommandContext>
    {
        public static Random random = new Random();

        [Command("8ball")]
        [Summary("Ask a yes or no question to the Sarcastic 8 Ball.")]
        public async Task EightBallAsync([Remainder] string question = null)
        {
            await ReplyAsync("The Magic 8 Ball says: " + GetRandomLineInFile(@"8ball.txt"));
        }

        [Command("flip")]
        [Summary("Flips a coin, resulting in Heads or Tails.")]
        public async Task FlipAsync([Remainder] string filler = null)
        {
            await ReplyAsync("The Coin Flip of Destiny has chosen " + (random.Next(2) == 1 ? "Heads" : "Tails") + "!");
        }

        [Command("rate")]
        [Summary("Rates something out of 10")]
        public async Task RateAsync([Remainder] string subject = "")
        {
            double rating = random.Next(0, 100) / 10.0;
            if(subject.Equals("me"))
            {
                await ReplyAsync("Hmmm... you're a " + rating + "/10.");
                return;
            }

            await ReplyAsync("I rate " + (subject.ToLower().StartsWith("that ") || subject.ToLower().StartsWith("the")
                    || subject.Contains("@") || subject.ToLower().EndsWith("s") ? "" : "that ")
                + subject + (subject.Length > 0 ? " " : "") + rating + "/10.");
        }

        [Command("roll")]
        [Alias("dice")]
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

        [Name("Error")]
        [Summary("Provides various error messages")]
        public class Error : ModuleBase<SocketCommandContext>
        {
            [Command("error")]
            [Summary("Returns an error")]
            public async Task ErrorAsync()
            {
                await ReplyAsync(GetRandomErrorMessage("Error!"));
            }

            public static string GetRandomErrorMessage(string preface = "")
            {
                return (preface + " " + GetRandomLineInFile("errors.txt")).Trim();
            }

            [Command("error add")]
            [Summary("Adds a new error message")]
            public async Task AddErrorAsync([Remainder] string error)
            {
                AppendStringToFile("error.txt", error);
                await ReplyAsync("Error added!");
            }
        }

        [Group("food")]
        [Summary("Provides commands for selecting a place or thing to eat")]
        public class FoodCommands : ModuleBase<SocketCommandContext>
        {
            [Command]
            [Summary("Select a place to eat, using the extremely scientific method of choosing randomly.")]
            public async Task FoodAsync()
            {
                var food = Food.GetRandom();
                await ReplyAsync(food.Value, GameBot.TryUseTts(Context.User.Id));
            }

            [Command]
            [Summary("Retrieve a specific food, added by a given user")]
            public async Task GetFoodCreatorAsync(SocketUser creator)
            {
                await ReplyAsync(Food.GetRandomByCreator(creator.Username).Value, GameBot.TryUseTts(Context.User.Id));
            }

            [Command("add")]
            [Summary("Add a food to the list")]
            public async Task AddFoodAsync([Remainder] string foodString)
            {
                await AddFoodCreatorAsync(Context.User, foodString);
            }

            [Command("add")]
            [Summary("Add a food to the list")]
            public async Task AddFoodCreatorAsync(SocketUser creator, [Remainder] string foodString)
            {
                if (foodString != "")
                {
                    var food = new Food(foodString, creator.Username);
                    food.Save();
                    await ReplyAsync($"Added food to list for {creator.Username}");
                }
                else
                {
                    await ReplyAsync("You didn't even specify a food! Boooooo!");
                }
            }

            [Command("set creator")]
            [Summary("Resets the creator of a specified food or list of foods")]
            public async Task EditFoodCreator(SocketUser creator, params int[] foodIds)
            {
                var foods = Food.GetByIds(foodIds);

                foreach (Food j in foods)
                {
                    j.Creator = creator.Username;
                    j.Save();
                }

                await ReplyAsync($"Foods attributed to {creator.Username}");
            }

            [Command("help")]
            [Summary("Provides help text on how to use the `food` commands")]
            public async Task FoodHelpAsync()
            {
                await ReplyAsync("usage:\n```" +
                    "  +food [id] : Retrieves a specific food if ID is provided or a random one if left blank.\n" +
                    "  +food add [food] : Adds [food] to the food list.  Multiple words and punctuation are okay.```");
            }

            [Command("list")]
            [Summary("Sends a list of all available foods")]
            public async Task ListFoodsAsync()
            {
                var foods = Food.GetAll();

                await Context.Channel.SendFileAsync(ListToStream(foods), "foods.txt", "The current list of foods:");
            }

            [Command("list")]
            [Summary("Sends a list of all foods added by a user")]
            public async Task ListFoodsAsync(SocketUser creator)
            {
                var foods = Food.GetByCreator(creator.Username);

                await Context.Channel.SendFileAsync(ListToStream(foods), "foods.txt", $"Foods added by {creator.Username}:");
            }

            [Command("delete"), Alias("remove")]
            [Summary("Delete a specific food, at the given index")]
            public async Task DeleteFoodAsync(int i)
            {
                var food = Food.GetById(i);
                Food.DeleteById(i);
                await ReplyAsync($"Food {i} deleted.  Final telling: {food.Value}");
            }
        }

        [Group("insult")]
        public class InsultCommands : ModuleBase<SocketCommandContext>
        {
            static int lastId = -1;

            [Command]
            [Summary("Tell an inside insult that normal people will need explained.")]
            public async Task InsultAsync()
            {
                var insult = Insult.GetRandom();
                lastId = insult.Id;
                await ReplyAsync(insult.Value, GameBot.TryUseTts(Context.User.Id));
            }

            [Command]
            [Summary("Retrieve a specific insult, at the given index")]
            public async Task GetInsultAsync(int i)
            {
                await ReplyAsync(Insult.GetById(i).Value, GameBot.TryUseTts(Context.User.Id));
            }

            [Command]
            [Summary("Retrieve a specific insult, added by a given user")]
            public async Task GetInsultCreatorAsync(SocketUser creator)
            {
                await ReplyAsync(Insult.GetRandomByCreator(creator.Username).Value, GameBot.TryUseTts(Context.User.Id));
            }

            [Command("who")]
            [Alias("creator")]
            [Summary("Retrieve a specific insult, at the given index")]
            public async Task GetInsultCreatorAsync(int i)
            {
                var insult = Insult.GetById(i);
                await ReplyAsync($"Insult {insult.Id} was added by {insult.Creator}");
            }

            [Command("add")]
            [Summary("Add a insult to the list")]
            public async Task AddInsultAsync([Remainder] string insultString)
            {
                await AddInsultCreatorAsync(Context.User, insultString);
            }

            [Command("add")]
            [Summary("Add a insult to the list")]
            public async Task AddInsultCreatorAsync(SocketUser creator, [Remainder] string insultString)
            {
                while (insultString.StartsWith("+"))
                {
                    insultString = insultString.Substring(1);
                }

                if (insultString != "")
                {
                    var insult = new Insult(insultString, creator.Username);
                    insult.Save();
                    await ReplyAsync($"Added insult to list with ID {insult.Id}");
                }
                else
                {
                    await ReplyAsync("You didn't even make a insult! Boooooo!");
                }
            }

            [Command("set creator")]
            [Summary("Resets the creator of a specified insult or list of insults")]
            public async Task EditInsultCreator(SocketUser creator, params int[] insultIds)
            {
                var insults = Insult.GetByIds(insultIds);

                foreach (Insult j in insults)
                {
                    j.Creator = creator.Username;
                    j.Save();
                }

                await ReplyAsync($"Insults attributed to {creator.Username}");
            }

            [Command("help")]
            [Summary("Provides help text on how to use the `insult` commands")]
            public async Task InsultHelpAsync()
            {
                await ReplyAsync("usage:\n```" +
                    "  +insult [id] : Tells a specific insult if ID is provided or a random one if left blank.\n" +
                    "  +insult add [insult] : Adds [insult] to the insult list.  Multiple words and punctuation are okay.```");
            }

            [Command("list")]
            [Summary("Sends a list of all available insults, as a private message to the requester")]
            public async Task ListInsultsAsync()
            {
                var insults = Insult.GetAll();

                await Context.Channel.SendFileAsync(ListToStream(insults), "insults.txt", "The current list of insults:");
            }

            [Command("list")]
            [Summary("Sends a list of all insults added by a user")]
            public async Task ListInsultsAsync(SocketUser creator)
            {
                var insults = Insult.GetByCreator(creator.Username);

                await Context.Channel.SendFileAsync(ListToStream(insults), "insults.txt", $"Insults added by {creator.Username}:");
            }

            [Command("which"), Alias("last")]
            [Summary("Displays the ID of the last random insult")]
            public async Task WhichInsultAsync()
            {
                if (lastId == -1)
                {
                    await ReplyAsync("I haven't told a random insult yet!");
                    return;
                }
                await ReplyAsync("The last random insult was: " + lastId);
            }

            [Command("delete"), Alias("remove")]
            [Summary("Retrieve a specific insult, at the given index")]
            public async Task DeleteInsultAsync(int i)
            {
                var insult = Insult.GetById(i);
                Insult.DeleteById(i);
                await ReplyAsync($"Insult {i} deleted.  Final telling: {insult.Value}");
            }
        }

        [Group("joke")]
        public class JokeCommands : ModuleBase<SocketCommandContext>
        {
            static int lastId = -1;

            [Command]
            [Summary("Tell an inside joke that normal people will need explained.")]
            public async Task JokeAsync()
            {
                var joke = Joke.GetRandom();
                lastId = joke.Id;
                await ReplyAsync(joke.Value, GameBot.TryUseTts(Context.User.Id));
            }

            [Command]
            [Summary("Retrieve a specific joke, at the given index")]
            public async Task GetJokeAsync(int i)
            {
                await ReplyAsync(Joke.GetById(i).Value, GameBot.TryUseTts(Context.User.Id));
            }

            [Command]
            [Summary("Retrieve a specific joke, added by a given user")]
            public async Task GetJokeCreatorAsync(SocketUser creator)
            {
                await ReplyAsync(Joke.GetRandomByCreator(creator.Username).Value, GameBot.TryUseTts(Context.User.Id));
            }

            [Command("who")]
            [Alias("creator")]
            [Summary("Retrieve a specific joke, at the given index")]
            public async Task GetJokeCreatorAsync(int i)
            {
                var joke = Joke.GetById(i);
                await ReplyAsync($"Joke {joke.Id} was added by {joke.Creator}");
            }

            [Command("add")]
            [Summary("Add a joke to the list")]
            public async Task AddJokeAsync([Remainder] string jokeString)
            {
                await AddJokeCreatorAsync(Context.User, jokeString);
            }

            [Command("add")]
            [Summary("Add a joke to the list")]
            public async Task AddJokeCreatorAsync(SocketUser creator, [Remainder] string jokeString)
            {
                while (jokeString.StartsWith("+"))
                {
                    jokeString = jokeString.Substring(1);
                }

                if (jokeString != "")
                {
                    var joke = new Joke(jokeString, creator.Username);
                    joke.Save();
                    await ReplyAsync($"Added joke to list with ID {joke.Id}");
                }
                else
                {
                    await ReplyAsync("You didn't even make a joke! Boooooo!");
                }
            }

            [Command("set creator")]
            [Summary("Resets the creator of a specified joke or list of jokes")]
            public async Task EditJokeCreator(SocketUser creator, params int[] jokeIds)
            {
                var jokes = Joke.GetByIds(jokeIds);

                foreach(Joke j in jokes)
                {
                    j.Creator = creator.Username;
                    j.Save();
                }

                await ReplyAsync($"Jokes attributed to {creator.Username}");
            }

            [Command("help")]
            [Summary("Provides help text on how to use the `joke` commands")]
            public async Task JokeHelpAsync()
            {
                await ReplyAsync("usage:\n```" +
                    "  +joke [id] : Tells a specific joke if ID is provided or a random one if left blank.\n" +
                    "  +joke add [joke] : Adds [joke] to the joke list.  Multiple words and punctuation are okay.```");
            }

            [Command("list")]
            [Summary("Sends a list of all available jokes, as a private message to the requester")]
            public async Task ListJokesAsync()
            {
                var jokes = Joke.GetAll();

                await Context.Channel.SendFileAsync(ListToStream(jokes), "jokes.txt", "The current list of jokes:");
            }

            [Command("list")]
            [Summary("Sends a list of all jokes added by a user")]
            public async Task ListJokesAsync(SocketUser creator)
            {
                var jokes = Joke.GetByCreator(creator.Username);

                await Context.Channel.SendFileAsync(ListToStream(jokes), "jokes.txt", $"Jokes added by {creator.Username}:");
            }

            [Command("which"), Alias("last")]
            [Summary("Displays the ID of the last random joke")]
            public async Task WhichJokeAsync()
            {
                if(lastId == -1)
                {
                    await ReplyAsync("I haven't told a random joke yet!");
                    return;
                }
                await ReplyAsync("The last random joke was: " + lastId);
            }
            
            [Command("delete"), Alias("remove")]
            [Summary("Retrieve a specific joke, at the given index")]
            public async Task DeleteJokeAsync(int i)
            {
                var joke = Joke.GetById(i);
                Joke.DeleteById(i);
                await ReplyAsync($"Joke {i} deleted.  Final telling: {joke.Value}");
            }
        }

        [Group("rip")]
        public class RipCommands : ModuleBase<SocketCommandContext>
        {
            static int lastId = -1;

            [Command]
            [Summary("Write an obituary.")]
            public async Task RipAsync()
            {
                var rip = Rip.GetRandom();
                lastId = rip.Id;
                await ReplyAsync(rip.Value, GameBot.TryUseTts(Context.User.Id));
            }

            [Command]
            [Summary("Retrieve a specific rip, at the given index")]
            public async Task GetRipAsync(int i)
            {
                await ReplyAsync(Rip.GetById(i).Value, GameBot.TryUseTts(Context.User.Id));
            }

            [Command]
            [Summary("Retrieve a specific rip, added by a given user")]
            public async Task GetRipCreatorAsync(SocketUser creator)
            {
                await ReplyAsync(Rip.GetRandomByCreator(creator.Username).Value, GameBot.TryUseTts(Context.User.Id));
            }

            [Command("who")]
            [Alias("creator")]
            [Summary("Retrieve a specific rip, at the given index")]
            public async Task GetRipCreatorAsync(int i)
            {
                var rip = Rip.GetById(i);
                await ReplyAsync($"Rip {rip.Id} was added by {rip.Creator}");
            }

            [Command("add")]
            [Summary("Add a rip to the list")]
            public async Task AddRipAsync([Remainder] string ripString)
            {
                await AddRipCreatorAsync(Context.User, ripString);
            }

            [Command("add")]
            [Summary("Add a rip to the list")]
            public async Task AddRipCreatorAsync(SocketUser creator, [Remainder] string ripString)
            {
                while (ripString.StartsWith("+"))
                {
                    ripString = ripString.Substring(1);
                }

                if (ripString != "")
                {
                    var rip = new Rip(ripString, creator.Username);
                    rip.Save();
                    await ReplyAsync($"Added rip to list with ID {rip.Id}");
                }
                else
                {
                    await ReplyAsync("You didn't even make a rip! Boooooo!");
                }
            }

            [Command("set creator")]
            [Summary("Resets the creator of a specified rip or list of rips")]
            public async Task EditRipCreator(SocketUser creator, params int[] ripIds)
            {
                var rips = Rip.GetByIds(ripIds);

                foreach (Rip j in rips)
                {
                    j.Creator = creator.Username;
                    j.Save();
                }

                await ReplyAsync($"Rips attributed to {creator.Username}");
            }

            [Command("help")]
            [Summary("Provides help text on how to use the `rip` commands")]
            public async Task RipHelpAsync()
            {
                await ReplyAsync("usage:\n```" +
                    "  +rip [id] : Tells a specific rip if ID is provided or a random one if left blank.\n" +
                    "  +rip add [rip] : Adds [rip] to the rip list.  Multiple words and punctuation are okay.```");
            }

            [Command("list")]
            [Summary("Sends a list of all available rips, as a private message to the requester")]
            public async Task ListRipsAsync()
            {
                var rips = Rip.GetAll();

                await Context.Channel.SendFileAsync(ListToStream(rips), "rips.txt", "The current list of rips:");
            }

            [Command("list")]
            [Summary("Sends a list of all rips added by a user")]
            public async Task ListRipsAsync(SocketUser creator)
            {
                var rips = Rip.GetByCreator(creator.Username);

                await Context.Channel.SendFileAsync(ListToStream(rips), "rips.txt", $"Rips added by {creator.Username}:");
            }

            [Command("which"), Alias("last")]
            [Summary("Displays the ID of the last random rip")]
            public async Task WhichRipAsync()
            {
                if (lastId == -1)
                {
                    await ReplyAsync("I haven't told a random rip yet!");
                    return;
                }
                await ReplyAsync("The last random rip was: " + lastId);
            }

            [Command("delete"), Alias("remove")]
            [Summary("Retrieve a specific rip, at the given index")]
            public async Task DeleteRipAsync(int i)
            {
                var rip = Rip.GetById(i);
                Rip.DeleteById(i);
                await ReplyAsync($"Rip {i} deleted.  Final telling: {rip.Value}");
            }
        }

        protected static string GetRandomLineInFile(string localpath, ref int lastId)
        {
            string path = GameBot.GetPath(localpath);

            var options = File.ReadAllLines(path);

            if(options.Count() == 0)
            {
                if(localpath == "errors.txt")
                {
                    return "How can you not even have error messages?!";
                }

                return Error.GetRandomErrorMessage("This file is empty!");
            }

            int id = random.Next(options.Length);
            string msg = options[id];
            lastId = id + 1;

            return msg;
        }

        protected static string GetRandomLineInFile(string path)
        {
            int id = -1;
            return GetRandomLineInFile(path, ref id);
        }

        protected static string GetLineInFile(string path, int i)
        {
            path = GameBot.GetPath(path);

            var options = File.ReadAllLines(path);
            
            if (options.Length == 0)
            {
                return Error.GetRandomErrorMessage("This file is empty!");
            }

            if(i < 0 || i > options.Length)
            {
                return Error.GetRandomErrorMessage("Hey!  That's not a real line number!");
            }

            string msg = options[i];

            return msg; 
        }

        protected static void AppendStringToFile(string path, string msg, bool isNewLine = true)
        {
            var writer = File.AppendText(GameBot.GetPath(path));
            writer.Write((isNewLine ? "\n" : "") + msg);
            writer.Flush();
            writer.Close();
        }

        protected static Stream ListToStream<T>(List<T> items){
            MemoryStream ms = new MemoryStream();
            TextWriter tw = new StreamWriter(ms);
            foreach (var item in items)
            {
                tw.WriteLine(item.ToString());
            }
            tw.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }
    }
}
