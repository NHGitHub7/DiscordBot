using DiscordBot.Helper;
using DiscordBot.DB;
using DSharpPlus;
using System.Threading.Tasks;
using System;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DiscordBot
{
  class Program
  {
    static void Main(string[] args)
    {
      Database.Init_Database();
      Database.defaultSetup();

      //MainAsync().GetAwaiter().GetResult();
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

      discord.MessageCreated += async (s, e) =>
        {
            response = messageDistributor.GetMessage(e).ToString();
            await e.Message.RespondAsync(response);
        };

      await discord.ConnectAsync();
      await Task.Delay(-1);
    }
  }
}
