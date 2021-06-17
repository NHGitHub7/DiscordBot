using DiscordBot.Helper;
using DiscordBot.DB;
using DSharpPlus;
using System.Threading.Tasks;
using System;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DiscordBot.MusicBot;

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
        TokenType = TokenType.Bot
      });

      var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
      {
        StringPrefixes = new[] { "!" }
      });
      commands.RegisterCommands<LavaLinkCommands>();
      commands.RegisterCommands<CustomCommands>();

      discord.MessageCreated += async (s, e) =>
        {
          response = messageDistributor.GetMessage(e).ToString();
          await e.Message.RespondAsync(response);
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
