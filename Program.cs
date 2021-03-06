using DiscordBot.Helper;
using DiscordBot.DB;
using DiscordBot.Swearwords;
using DSharpPlus;
using System.Threading.Tasks;
using System.Threading;
using System;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext.Attributes;
using System.Text.Json;
using System.Text.Json.Serialization;
using DiscordBot.Rolemanager;
using System.Collections.Specialized;
using System.Threading.Channels;
using Newtonsoft.Json.Linq;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DiscordBot.MusicBot;
using DiscordBot.STFU;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;

namespace DiscordBot
{
  class Program
  {
    /*
     * Start Method in C# Console App
     */
    static void Main(string[] args)
    {
      ConfigurationHelper configHelper = new ConfigurationHelper();
      Model.Versioning version_config = configHelper.GetVersion();
      Model.OAuthorization oauth_config = configHelper.GetOAuthValue();
      Model.DB_Access db_config = configHelper.GetDBAccessValues();

      if (typeof(object).IsInstanceOfType(version_config) && typeof(object).IsInstanceOfType(oauth_config) && typeof(object).IsInstanceOfType(db_config))
      {
        Database.Init_Database();
        Database.defaultSetup();
        Blacklist.init();

        MainAsync().GetAwaiter().GetResult();
      }
      else
      {
        Console.WriteLine("Check your config, maybe something is missing!");
      }
    }

    /*
     * async Task Main Method
     */
    static async Task MainAsync()
    {
      ConfigurationHelper configurationHelper = new ConfigurationHelper();

      string response = String.Empty;

      var discord = new DiscordClient(new DiscordConfiguration()
      {
        Token = configurationHelper.GetOAuthValue().Token,
        TokenType = TokenType.Bot,
        Intents = DiscordIntents.All
      });

      /*
       * Command Prefix you need, to use the Command.
       */
      var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
      {
        StringPrefixes = new[] { "!" }
      });
      commands.RegisterCommands<LavaLinkCommands>();
      commands.RegisterCommands<CustomCommands>();
      commands.RegisterCommands<RoleCommands>();
      commands.RegisterCommands<Swearwords.Commands>();
      commands.RegisterCommands<MuteCommand>();

      RoleEventReactions roleEvents = new RoleEventReactions();


      discord.MessageCreated += async (s, e) =>
        {
          if (e.Channel.IsPrivate == true && e.Author.IsBot == false)
          {
            await roleEvents.ReactOnUserMessage(e, discord);
          }
          else if ((e.Author.IsBot == false) && (Blacklist.is_swearword(e.Message.Content.ToLower(), e)))
          {
            await Blacklist.strike_user(e);
          }
        };
      /*
       * Event that reacts on User Join in your Guild.
       */
      discord.GuildMemberAdded += async (s, e) =>
      {
        await roleEvents.ReactOnUserJoin(s, e);
      };

      /*
       * Event that reacts when the Guild is ready.
       */
      discord.GuildAvailable += async (s, e) =>
      {
        await roleEvents.SetRolesToDB(discord);
      };
      var endpoint = new ConnectionEndpoint
      {
        Hostname = "127.0.0.1",
        Port = 2333
      };

      var lavalinkConfig = new LavalinkConfiguration
      {
        Password = "youshallnotpass",
        RestEndpoint = endpoint,
        SocketEndpoint = endpoint
      };

      var lavalink = discord.UseLavalink();

      await discord.ConnectAsync();

      await lavalink.ConnectAsync(lavalinkConfig);


      await Task.Delay(-1);


    }
  }
}
