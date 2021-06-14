using DiscordBot.Helper;
using DiscordBot.DB;
using DSharpPlus;
using System.Threading.Tasks;
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
using DSharpPlus.CommandsNext;
namespace DiscordBot
{
  class Program
  {
    static void Main(string[] args)
    {

      Database.Init_Database();
      Database.defaultSetup();
      var tmp = Database.runSQL("SELECT * FROM CustomRoles");

      foreach (var i in tmp)
      {
        foreach (var j in i)
        {
          Console.WriteLine(j);
        }
      }


      MainAsync().GetAwaiter().GetResult();

    }
    static async Task MainAsync()
    {
      ConfigurationHelper configurationHelper = new ConfigurationHelper();
      DcMessageDistributor messageDistributor = new DcMessageDistributor();

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
      commands.RegisterCommands<RoleCommands>();

      RoleEventReactions roleEvents = new RoleEventReactions();

      discord.MessageCreated += async (s, e) =>
        {
            response = messageDistributor.GetMessage(e).ToString();
            await e.Message.RespondAsync(response);
            await roleEvents.ReactOnUserMessage(s, e, discord);
        };
      /*
       * Event that reacts on User Join in your Guild.
       */
      discord.GuildMemberAdded += async (s, e) =>
      {
        await roleEvents.ReactOnUserJoin(s, e);
      };

      await discord.ConnectAsync();
      await Task.Delay(-1);

    }
  }
}
