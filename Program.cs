using DiscordBot.Helper;
using DiscordBot.DB;
using DSharpPlus;
using System.Threading.Tasks;
using System;
using DSharpPlus.EventArgs;


namespace DiscordBot
{
  class Program
  {
    static void Main(string[] args)
    {
      var db = new Database();
      db.defaultSetup();
      var tmp = db.runSQL("SELECT VESION()");

      if (tmp[tmp.Length - 1].ToString() == "Error")
      {
        Console.WriteLine("Check your SQL Syntax");
      }
      else
      {
        foreach (var i in tmp)
        {
          Console.WriteLine(i);
        }
        {

        }
      }


      //MainAsync().GetAwaiter().GetResult();
      //GuildmemberTask().GetAwaiter().GetResult();

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

      DiscordChannel[] arrayChannels;
      DiscordGuild[] arrayGuilds;
      DiscordRole[] arrayRoles;


      discord.MessageCreated += async (s, e) =>
        {
            response = messageDistributor.GetMessage(e).ToString();
            await e.Message.RespondAsync(response);
        };
      await discord.ConnectAsync();
      await Task.Delay(-1);
    }
  }

  public class RoleCommands : BaseCommandModule
  {
    [Command("beispiel")]
    public async Task AddKeyCodeToRole(CommandContext ctx, string rolename, string keycode)
    {

    }

  }
}
