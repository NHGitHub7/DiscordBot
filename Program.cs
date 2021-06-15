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
          /*
           * If you have Questions ask me, unable to use messageDistributor because it is not an async Task,
           * i need the await for that GrantRoleAsync Task vgl. Rolemanager/RoleEventReactions.cs line 56.
           */
          if (e.Channel.IsPrivate == true && e.Author.IsBot == false)
          {
            await roleEvents.ReactOnUserMessage(e, discord);
            await e.Message.RespondAsync("You will receive your Role.");
          }
          else
          {
            response = messageDistributor.GetMessage(e).ToString();
            await e.Message.RespondAsync(response);
          }
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
