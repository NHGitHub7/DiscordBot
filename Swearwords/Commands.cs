using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.DB;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordBot.Swearwords
{
  public class Commands : BaseCommandModule
  {
    [Command("cussAdd"), RequirePermissions(Permissions.Administrator)]
    public async Task add_cuss_to_db(CommandContext ctx, string cuss_input)
    {
      string cuss_clean = cuss_input.ToLower();
      if (cuss_exists(cuss_clean))
      {
        await ctx.RespondAsync($"{cuss_clean} already exists !");
      }
      else
      {
        string query = "INSERT INTO swearwords(word) " +
          $"VALUES ('{cuss_clean}')";
        Database.runScalar(query);
        Blacklist.init();
        await ctx.RespondAsync($"Cuss '{cuss_input}' Added.");
      }
    }

    [Command("cussRm"), RequirePermissions(Permissions.Administrator)]
    public async Task rm_cuss_from_db(CommandContext ctx, string cuss)
    {
      string query = "DELETE FROM swearwords " +
        $"WHERE word LIKE '{cuss}'";
      Database.runScalar(query);
      await ctx.RespondAsync("Cuss removed;");
    }

    [Command("cussList")]
    public async Task list_cusses_from_db(CommandContext ctx)
    {
      List<string> cuss_list = Database.get_swearwords();
      string msg = "";
      foreach (var i in cuss_list)
      {
        msg += $"`{i}`\n";
      }
      await ctx.RespondAsync(msg);
    }

    [Command("cussHelp")]
    public async Task print_cuss_help(CommandContext ctx)
    {
      string help = "" +
        "**!cussHelp** Shows this message\n" +
        "**!cussList** Lists all registered cusses\n" +
        "**!cussAdd <word>** Adds the provided word to your registered cusses\n" +
        "**!cussRm <word>** Removes the provided word from your registered cusses\n";
      await ctx.RespondAsync(help);
    }

    static bool cuss_exists(string cuss)
    {
      string query = "SELECT word " +
        "FROM swearwords " +
        $"WHERE word like '{cuss}'";
      object db_val = Database.runScalar(query);
      return db_val != null;
    }
  }
}
