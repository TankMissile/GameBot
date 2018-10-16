using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameBot.Modules
{
    [Group("dnd")]
    [Alias("d&d")]
    [Name("DnD")]
    [Summary("A group of commands designed to assist playing Dungeons and Dragons")]
    public class DND : ModuleBase<SocketCommandContext>
    {
        [Group("setup")]
        [Summary("Commands designed around setting up your character")]
        public class CharacterSetup : ModuleBase<SocketCommandContext>
        {
            [Command("stats")]
            [Summary("Rolls 4d6 6 times, dropping the lowest d6 of each.  This is designed for rolling your stat values.")]
            public async Task RollStatsAsync()
            {
                string msg = "Your stat values are...\n";
                List<int> stats = new List<int>();
                for(int i = 0; i < 6; i++)
                {
                    List<int> rolls = new List<int>();
                    for(int j = 0; j < 4; j++)
                    {
                        rolls.Add(RNG.random.Next(6) + 1);
                    }
                    rolls.Sort();

                    stats.Add(rolls[1] + rolls[2] + rolls[3]);
                }

                stats.Sort();
                stats.Reverse();

                foreach(int i in stats)
                {
                    msg += i + " ";
                }

                await ReplyAsync(msg);
            }
        }


        [Command("roll")]
        [Summary("Rolls a given die")]
        public async Task RollAsync(int numSides)
        {
            int roll = RNG.random.Next(numSides);
            string msg = "You roll a " + roll + "!";
            if (roll == 1)
            {
                msg += "  C R I T I C A L   F A I L U R E";
            }
            else if(roll == numSides)
            {
                msg += "  C R I T I C A L   S U C C E S S";
            }
            await ReplyAsync(msg);
        }

        [Command("roll")]
        [Summary("Rolls a given set of dice")]
        public async Task RollAsync(int numRolls, int numSides)
        {
            int sum = 0;
            string msg = "You begin rolling...\n```";
            List<int> rolls = new List<int>();

            for (int i = 0; i < numRolls; i++)
            {
                int roll = RNG.random.Next(numSides) + 1;
                sum += roll;
                msg += roll + "! ";
                rolls.Add(roll);
            }

            if (numSides == 10)
            {
                msg += "\n\nAs digits: ";
                foreach (int i in rolls)
                {
                    msg += (i == 10 ? 0 : i);
                }
            }

            msg += "\n\nTotal: " + sum;
            if (sum == numRolls)
            {
                msg += "  C R I T I C A L   F A I L U R E";
            }
            else if (sum == numRolls * numSides)
            {
                msg += "  C R I T I C A L   S U C C E S S";
            }
            msg += "```";
            await ReplyAsync(msg);
        }


        [Command("roll"), Priority(-1)]
        [Summary("Rolls a given set of dice, in the format 1d6")]
        public async Task RollAsync(string dieString)
        {
            Regex hasD = new Regex("^([1-9][0-9]*)?d[1-9][0-9]*$");
            if (hasD.IsMatch(dieString))
            {
                string[] ins = dieString.Split("d");
                Console.Write("rolls: " + ins[0] + ", Sides: " + ins[1]);
                int numRolls = 1;
                if(ins[0].Length > 0)
                {
                    numRolls = Int32.Parse(ins[0]);
                }

                int numSides = Int32.Parse(ins[1]);

                int sum = 0;
                string msg = "You begin rolling...\n```";
                List<int> rolls = new List<int>();

                for(int i = 0; i < numRolls; i++)
                {
                    int roll = RNG.random.Next(numSides) + 1;
                    sum += roll;
                    msg += roll + "! ";
                    rolls.Add(roll);
                }

                if(numSides == 10)
                {
                    msg += "\n\nAs digits: ";
                    foreach(int i in rolls)
                    {
                        msg += (i == 10 ? 0 : i);
                    }
                }

                msg += "\n\nTotal: " + sum;
                if(sum == numRolls)
                {
                    msg += "  C R I T I C A L   F A I L U R E";
                }
                else if (sum == numRolls * numSides)
                {
                    msg += "  C R I T I C A L   S U C C E S S";
                }
                msg += "```";
                await ReplyAsync(msg);
            }
            else
            {
                await ReplyAsync("Invalid input.  Must follow one of the following formats:\n" +
                    "```" +
                    "  +dnd roll RdS\n" +
                    "  +dnd roll dS\n" +
                    "  +dnd roll S\n" +
                    "  +dnd roll R S" +
                    "```" +
                    "where R is the number of rolls (>0) and S is the number of sides (>0)");
            }
        }
    }
}
