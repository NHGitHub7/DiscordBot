using DiscordBot.Helper;
using DiscordBot.DB;
using DSharpPlus;
using System.Threading.Tasks;
using System;

namespace DiscordBot
{
  class Program
  {
    static void Main(string[] args)
    {
      var db = new Database();
      Console.WriteLine(db.runSQL("SELECT VERSION()"));
      //MainAsync().GetAwaiter().GetResult();
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
