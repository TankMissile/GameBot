using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace GameBot.Modules
{
    //the class has to be public, you nimrod
    public class TextModifiers : ModuleBase<SocketCommandContext>
    {
        [Group("mock")]
        public class Mock : ModuleBase<SocketCommandContext>
        {
            [Command, Priority(-1)]
            public async Task MockAsync([Remainder] string text)
            {
                char[] characters = ConvertCharacters(text);

                await ReplyAsync(String.Join("", characters));
            }

            [Command("-s")]
            public async Task MockSpacedAsync([Remainder] string text)
            {
                char[] characters = ConvertCharacters(text);

                await ReplyAsync(String.Join(" ", characters));
            }

            private char[] ConvertCharacters(string text)
            {
                char[] characters = text.ToCharArray();

                bool wasUppercase = false;
                short sinceFlip = 0;
                for (int i = 0; i < characters.Length; i++)
                {
                    if (Char.IsLetter(characters[i])) {
                        if (sinceFlip > 1)
                        {
                            sinceFlip = 1;
                            if (!wasUppercase)
                            {
                                characters[i] = Char.ToUpper(characters[i]);
                            }
                            wasUppercase = !wasUppercase;
                            continue;
                        }

                        if (RNG.random.NextDouble() >= 0.5)
                        {
                            if (!wasUppercase)
                            {
                                sinceFlip = 1;
                            }
                            else
                            {
                                sinceFlip++;
                            }
                            wasUppercase = true;
                            characters[i] = Char.ToUpper(characters[i]);
                        }
                        else
                        {
                            if (wasUppercase)
                            {
                                sinceFlip = 1;
                            }
                            else
                            {
                                sinceFlip++;
                            }
                            wasUppercase = false;
                        }
                    }
                }

                return characters;
            }
        }
    }
}
