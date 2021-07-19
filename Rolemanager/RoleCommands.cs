using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus;
using System.Threading.Tasks;
using DiscordBot.DB;

namespace DiscordBot.Rolemanager
{
  public class RoleCommands : BaseCommandModule
  {

    [Command("saverolewithpw"), RequirePermissions(Permissions.Administrator)]
    public async Task AddKeyCodeToRole(CommandContext ctx, string rolename, string password)
    {
      await SaveRoleToDB(rolename, password, ctx);
    }
    [Command("updaterolewithpw"), RequirePermissions(Permissions.Administrator)]
    public async Task UpdateKeyCodeFromRole(CommandContext ctx, string rolename, string password)
    {
      await UpdateRoleInDB(rolename, password, ctx);
    }
    [Command("deleterole"), RequirePermissions(Permissions.Administrator)]
    public async Task DeleteRole(CommandContext ctx, string rolename, string password)
    {
      await DeleteRoleInDB(rolename, password, ctx);
    }
    private async Task SaveRoleToDB(string customrolename, string custompassword, CommandContext ctx)
    {
      string sqlQuery = $"SELECT * FROM customroles WHERE rolename='{customrolename}'";
      var returnsql = Database.runSQL(sqlQuery);
      if (returnsql.Count != 0)
      {
        await ctx.RespondAsync("This Role already exists with a Password. If you want to update the Password use !updaterolewithpw {rolename} {password}");
      }
      else
      {
        sqlQuery = $"INSERT INTO customroles (rolename, password) VALUES ('{customrolename}', '{custompassword}')";
        Database.runSQL(sqlQuery);
      }
    }

    private async Task UpdateRoleInDB(string customrolename, string customkeycode, CommandContext ctx)
    {
      string sqlQuery = $"UPDATE customroles SET password = '{customkeycode}' WHERE rolename='{customrolename}'";
      Database.runSQL(sqlQuery);
    }

    private async Task DeleteRoleInDB(string customrolename, string custompassword, CommandContext ctx)
    {
      string sqlQuery = $"DELETE FROM customroles WHERE rolename = '{customrolename}' and password = '{custompassword}'";
      Database.runSQL(sqlQuery);
    }
  }
}
