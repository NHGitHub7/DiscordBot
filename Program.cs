﻿using DiscordBot.Helper;
using DSharpPlus;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            ConfigurationHelper configurationHelper = new ConfigurationHelper();

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = configurationHelper.GetOAuthValue().Token,
                TokenType = TokenType.Bot
            });

            discord.MessageCreated += async (s, e) =>
            {
                if (e.Message.Content.ToLower().StartsWith("ping"))
                    await e.Message.RespondAsync("pong!");
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
