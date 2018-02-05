﻿using Discord;
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

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection().AddSingleton(_client).AddSingleton(_commands).BuildServiceProvider();

            string botToken;
            if (File.Exists(@"token.txt"))
            {
                botToken = System.IO.File.ReadAllText(@"token.txt");

                //event subscriptions
                _client.Log += Log;

                await RegisterCommandsAsync();

                await _client.LoginAsync(Discord.TokenType.Bot, botToken);

                await _client.StartAsync();
            }
            else
            {
                Console.WriteLine("Error!  No bot token.  Create a text file in the root directory named token.txt containing your bot token.");
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
            await _commands.AddModulesAsync(typeof(GameBot).GetTypeInfo().Assembly); //this is the problem of whatever problem you are having
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
                }
            }
        }
    }
}
