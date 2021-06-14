using DiscordBot.Helper;
using DiscordBot.DB;
using DSharpPlus;
using System.Threading.Tasks;
using System;
using System.Reflection.Metadata.Ecma335;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using DSharpPlus.CommandsNext;
using MySqlX.XDevAPI;
using Client = MySql.Data.MySqlClient.Memcached.Client;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext.Attributes;
using System.Text.Json;
using System.Text.Json.Serialization;
using DiscordBot.Rolemanager;
using System.Collections.Specialized;
using System.Threading.Channels;
using Newtonsoft.Json.Linq;


namespace DiscordBot
{
  class Program
  {
    static void Main(string[] args)
    {
      var db = new Database();
      db.defaultSetup();
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
          if (e.Message.Content.ToLower().StartsWith("!getdiscord"))
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
        TokenType = TokenType.Bot,
        Intents = DiscordIntents.All
      });
      RoleEventReactions roleEvents = new RoleEventReactions();
      /*
       * Command Prefix you need, to use the Command.
       */
      var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
      {
        StringPrefixes = new[] { "!" }
      });
      commands.RegisterCommands<RoleCommands>();
      /*
       * Event that reacts on User Join in your Guild.
       */
      discord.GuildMemberAdded += async (s, e) =>
      {
        await roleEvents.ReactOnUserJoin(s, e);
      };
      /*
       * Event that reacts on Message written from User.
       */
      discord.MessageCreated += async (s, e) =>
      {
        await roleEvents.ReactOnUserMessage(s, e, discord);
      };
      await discord.ConnectAsync();
      await Task.Delay(-1);
    }
  }
}
