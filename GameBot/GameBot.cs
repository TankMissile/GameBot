using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace GameBot
{
    class GameBot
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        static void Main(string[] args) => new GameBot().MainAsync().GetAwaiter().GetResult();
        
        public static ulong lastTtsUser = 0;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection().AddSingleton(_client).AddSingleton(_commands).BuildServiceProvider();

            string botToken;
            string tokenPath = GetPath("token.txt");
            if (File.Exists(tokenPath))
            {
                botToken = File.ReadAllText(tokenPath);

                if (botToken != "") {
                    //event subscriptions
                    _client.Log += Log;

                    await RegisterCommandsAsync();

                    await _client.LoginAsync(TokenType.Bot, botToken);

                    await _client.StartAsync();

                    await _client.SetGameAsync("+help");
                }
                else
                {
                    ExitNoToken(tokenPath);
                }
            }
            else
            {
                ExitNoToken(tokenPath, true);
            }

            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg.ToString());

            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(typeof(GameBot).GetTypeInfo().Assembly);
        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message is null) return;

            int argPos = 0;


            //Call commands if the + prefix is used or the bot is tagged with @
            if (message.HasStringPrefix("+", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                    await context.Channel.SendMessageAsync(Modules.RNG.Error.GetRandomErrorMessage("Something weird happened!"));
                }
            }
        }

        public static string GetPath(string localPath)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GameBot\\", localPath);
            if (!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(Path.GetDirectoryName(path));

            if (Path.HasExtension(path) && !File.Exists(path))
            {
                if (File.Exists(localPath))
                {
                    File.Copy(localPath, path);
                }
                else
                {
                    File.Create(path);
                }
            }

            return path;
        }

        public static bool TryUseTts(ulong user)
        {
            if (user == lastTtsUser)
            {
                return false;
            }

            lastTtsUser = user;
            return true;
        }

        private static void ExitNoToken(string tokenPath, bool create = false)
        {
            if(create) File.Create(tokenPath);
            Console.WriteLine("Error!  No bot token.  Enter your bot's token into the file " + tokenPath);
            Console.Write("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}
