using DiscordBot.Helper;
using DiscordBot.DB;
using DSharpPlus;
using System.Threading.Tasks;
using System;
using System.Reflection.Metadata.Ecma335;
using DSharpPlus.Entities;
using System.Collections.Generic;


namespace DiscordBot
{
  class Program
  {
    static void Main(string[] args)
    {
      //MainAsync().GetAwaiter().GetResult();
      GuildmemberTask().GetAwaiter().GetResult();
    }

    static async Task MainAsync()
    {
      ConfigurationHelper configurationHelper = new ConfigurationHelper();

      var discord = new DiscordClient(new DiscordConfiguration()
      {
        Token = configurationHelper.GetOAuthValue().Token,
        TokenType = TokenType.Bot
      });

      DiscordChannel[] arrayChannels;
      DiscordGuild[] arrayGuilds;
      DiscordRole[] arrayRoles;


      discord.MessageCreated += async (s, e) =>
        {

            if (e.Message.Content.ToLower().StartsWith("ping"))
                await e.Message.RespondAsync("pong");
            if (e.Message.Content.ToLower().StartsWith("getdiscord"))
            {

                arrayGuilds = new List<DiscordGuild>(discord.Guilds.Values).ToArray();
                foreach (var guild in arrayGuilds)
                {

                    //arrayChannels = new List<DiscordChannel>(guild.Channels.Values).ToArray();

                    //foreach (var channel in arrayChannels)
                    //{

                    //    await e.Message.RespondAsync(channel.Name);
                    //}
                }
            }
        };

      await discord.ConnectAsync();
      await Task.Delay(-1);

    }

    static async Task GuildmemberTask()
    {
        ConfigurationHelper configurationHelper = new ConfigurationHelper();

        var discord = new DiscordClient(new DiscordConfiguration()
        {
            Token = configurationHelper.GetOAuthValue().Token,
            TokenType = TokenType.Bot
        });
        DiscordChannel[] arrayChannels;
        DiscordGuild[] arrayGuilds;
        DiscordRole[] arrayRoles;
        discord.GuildMemberAdded += async (s, e) =>
        {
            arrayGuilds = new List<DiscordGuild>(discord.Guilds.Values).ToArray();
            foreach (var guild in arrayGuilds)
            {
                arrayRoles = new List<DiscordRole>(guild.Roles.Values).ToArray();
                foreach (var role in arrayRoles)
                {
                    if (role.Name == "TestF")
                        await e.Member.GrantRoleAsync(role);
                }
            } //await e.Member.GrantRoleAsync();

        };
        await discord.ConnectAsync();
        await Task.Delay(-1);
        }
  }
}
