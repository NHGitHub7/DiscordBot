using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus;
using System.Threading.Tasks;
using DiscordBot.DB;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DiscordBot.Rolemanager
{
  public class RoleCommands : BaseCommandModule
  {
    [Command("listroles"), RequirePermissions(Permissions.Administrator), Description("Command Usage: !listroles Info: List of all current Roles + passwords and if they are active.")]
    public async Task ListAllCustomRoles(CommandContext ctx)
    {
      await ListAllCustomRolesFromDB(ctx);
    }
    
    [Command("createrolewithpw"), RequirePermissions(Permissions.Administrator), Description("Command Usage: !createrolewithpw {rolename} {password} {active default: true} Info: Saves new Role with a password to your DB.")]
    public async Task AddKeyCodeToRole(CommandContext ctx, string rolename, string password, bool active = true)
    {
       await SaveRoleToDB(rolename, password, active,  ctx);
    }
    [Command("updaterolewithpw"), RequirePermissions(Permissions.Administrator), Description("Command Usage: !updaterolewithpw {rolename} {password} {active default: true} Info: Updates Password of existing Role in your DB.")]
    public async Task UpdateKeyCodeFromRole(CommandContext ctx, string rolename, string password, bool active = true)
    {
      await UpdateRoleInDB(rolename, password, active, ctx);
    }
    [Command("deleterole"), RequirePermissions(Permissions.Administrator), Description("Command Usage: !deleterole {rolename} {password} Info: It deletes a custom Role in your DB with its password.")]
    public async Task DeleteRole(CommandContext ctx, string rolename, string password)
    {
        await DeleteRoleInDB(rolename, password, ctx);
    }
    [Command("setactive"), RequirePermissions(Permissions.Administrator), Description("Command Usage: !setactive {rolename} {active:true/false} Info: Activate a Role or deactivate, if its active the password can be used for new Members else it can't.")]
    public async Task ActivateRoleOrDeactivate(CommandContext ctx, string rolename, bool active = true)
    {
      await ActivateRoleOrDeactivateInDB(rolename, active, ctx);
    }

    private async Task ListAllCustomRolesFromDB(CommandContext ctx)
    {
      string sqlQuery = "SELECT rolename, password, active FROM customroles";
      var returnsql = Database.runSQL(sqlQuery);
      for (var i = 0; i < returnsql.Count; i++)
      {

        await ctx.RespondAsync("Rolename: " + returnsql[i][0].ToString() + " \r\nPassword: " + returnsql[i][1].ToString() + " \r\nActive: " + returnsql[i][2].ToString());
        
      };
    }
    private async Task SaveRoleToDB(string customrolename, string custompassword, bool active, CommandContext ctx)
    {
      string sqlQuery = $"SELECT * FROM customroles WHERE rolename='{customrolename}'";
      var returnsql = Database.runSQL(sqlQuery);
      if (returnsql.Count != 0)
      {
        await ctx.RespondAsync("This Role already exists with a Password. If you want to update the Password use !updaterolewithpw {rolename} {password}");
      }
      else
      {
        sqlQuery = $"INSERT INTO customroles (rolename, password, active) VALUES ('{customrolename}', '{custompassword}', {active})";
        Database.runSQL(sqlQuery);
      }
    }

    private async Task UpdateRoleInDB(string customrolename, string custompassword, bool active, CommandContext ctx)
    {
      bool exists = CheckIfRoleExists(customrolename);
      if (exists)
      {
        string sqlQuery = $"UPDATE customroles SET password = '{custompassword}', active = {active} WHERE rolename='{customrolename}'";
        Database.runSQL(sqlQuery);
        await ctx.RespondAsync($"{customrolename} is now updated!");
      }
      else if (exists == false)
      {
        await ctx.RespondAsync("This Role does not exist. Maybe you want to create the Role first with !createrolewithpw {rolename} {password} {active:true/false} ");
      }
    }

    private async Task DeleteRoleInDB(string customrolename, string custompassword, CommandContext ctx)
    {
      bool exists = CheckIfRoleExists(customrolename);
      if (exists)
      {
        string sqlQuery = $"DELETE FROM customroles WHERE rolename = '{customrolename}'";
        Database.runSQL(sqlQuery);
        await ctx.RespondAsync($"{customrolename} is now deleted!");
      } 
      else if (exists == false)
      {
        await ctx.RespondAsync("You can't delete a Role that does not exist.");
      }
    }

    private async Task ActivateRoleOrDeactivateInDB(string customrolename, bool active, CommandContext ctx)
    {
      bool exists = CheckIfRoleExists(customrolename);
      if (exists)
      {
        string sqlQuery = $"UPDATE customroles SET active = {active} WHERE rolename='{customrolename}'";
        Database.runSQL(sqlQuery);
        if (active)
        {
          await ctx.RespondAsync($"{customrolename} is now active!");
        }
        else if (active == false)
        {
          await ctx.RespondAsync($"{customrolename} is now deactivated!");
        }
      }
      else if (exists == false)
      {
        await ctx.RespondAsync("You can't update a Role that does not exist.");
      }
    }

    private bool CheckIfRoleExists(string customrolename)
    {
      string sqlQuery = $"SELECT rolename FROM customroles";
      var returnsql = Database.runSQL(sqlQuery);
      for (int i = 0; i < returnsql.Count; i++)
      {
        if (returnsql[i][0].ToString() == customrolename)
        {
          return true;
        }
      }
      return false;
    }
  }
}
