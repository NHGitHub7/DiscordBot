using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiscordBot.DB;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordBot.Swearwords
{
  public class Commands : BaseCommandModule
  {
    [Command("addcuss"), RequirePermissions(Permissions.Administrator)]
    public async Task add_cuss_to_db(CommandContext ctx, string cuss_input)
    {
      /*var reg = new Regex("/^[ !@#$%^&*()_+={};':\"\\|,.<>/?]*$/");
      if (reg.IsMatch(cuss_input))
      {
        await ctx.RespondAsync("No Special Chars allowed");
      }
      else
      { */
      string cuss_clean = cuss_input.ToLower();
      string query = "INSERT INTO swearwords(word) " +
        $"VALUES ('{cuss_clean}')";
      Database.runScalar(query);
      Blacklist.init();
      await ctx.RespondAsync($"Cuss '{cuss_input}' Added.");
      //}
    }

    [Command("rmcuss"), RequirePermissions(Permissions.Administrator)]
    public async Task rm_cuss_from_db(CommandContext ctx, string cuss)
    {
      string query = "DELETE FROM swearwords " +
        $"WHERE word LIKE '{cuss}'";
      Database.runScalar(query);
      await ctx.RespondAsync("Cuss removed;");
    }
  }
}
