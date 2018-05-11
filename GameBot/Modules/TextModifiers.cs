using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace GameBot.Modules
{
    //the class has to be public, you nimrod
    public class TextModifiers : ModuleBase<SocketCommandContext>
    {
        Random r = new Random();

        [Command("mock")]
        public async Task MockAsync(string flag, [Remainder] string text)
        {
            string sentence = "";
            string separator = "";

            if(flag == "-s")
            {
                separator = " ";
            }
            else
            {
                sentence += flag + " ";
            }

            sentence += text;

            char[] characters = sentence.ToCharArray();

            for(int i = 0; i < characters.Length; i++)
            {
                if (Char.IsLetter(characters[i]) && r.NextDouble() >= 0.5)
                {
                    characters[i] = Char.ToUpper(characters[i]);
                }
            }

            await ReplyAsync(String.Join(separator, characters));
        }
    }
}
