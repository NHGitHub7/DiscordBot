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
    [Command("cussAdd"), RequirePermissions(Permissions.Administrator), Description("Adds the provided word to your registered cusses")]
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

    [Command("cussRm"), RequirePermissions(Permissions.Administrator), Description("Removes the provided word from your registered cusses")]
    public async Task rm_cuss_from_db(CommandContext ctx, string word)
    {
      string query = "DELETE FROM swearwords " +
        $"WHERE word LIKE '{word}'";
      Database.runScalar(query);
      await ctx.RespondAsync("Cuss removed;");
    }

    [Command("cussList"), Description("Lists all registered cusses")]
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

    /**
     * Check if the specified cuss already exists in the DB
     */
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
