using DiscordBot.DB;
using DiscordBot.Model.DbTables;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot
{
  class CustomCommands : BaseCommandModule
  {
    #region Variables
    string commandName = String.Empty;
    string commandResponse = String.Empty;
    string user = String.Empty;
    string addToDb = String.Empty;
    #endregion

    /*
     * Method to add CustomCommand
     */
    [Command("ccAdd"), Description("Command to create your CustomCommand")]
    public async Task AddDatabaseEntry(CommandContext ctx, string commandName, [RemainingText] string commandResponse)
    {
      try
      {
        string user = ctx.User.Id.ToString();

        addToDb = $"INSERT INTO customcommands(CommandName, CommandResponse, DateCreated, CreatedBy) VALUES('{commandName}', '{commandResponse}', NOW(), '{user}')";

        Database.runSQL(addToDb);
      }
      catch (Exception exception)
      {
        await ctx.RespondAsync(exception.Message);
      }

      await ctx.RespondAsync("Added Command");
    }

    /*
     * Method to update your CommandResponse
     */
    [Command("ccUpdate"), Description("Command to update your CustomCommand")]
    public async Task UpdateDatabaseEntry(CommandContext ctx, string commandName, [RemainingText] string commandResponse)
    {
      try
      {
        string user = ctx.User.Id.ToString();

        addToDb = $"UPDATE customcommands SET CommandName = '{commandName}', CommandResponse = '{commandResponse}', DateModified = Now(), ModifiedBy = '{user}' WHERE CommandName = '{commandName}'";

        Database.runSQL(addToDb);
      }
      catch (Exception exception)
      {
        await ctx.RespondAsync(exception.Message);
      }

      await ctx.RespondAsync("Command changed");
    }

    /*
     * Method to delete CustomCommand
     */
    [Command("ccDelete"), Description("Command to delete your CustomCommand")]
    public async Task DeleteDatabaseEntry(CommandContext ctx, string commandName)
    {
      try
      {
        addToDb = $"DELETE FROM customcommands WHERE CommandName = '{commandName}'";

        Database.runSQL(addToDb);
      }
      catch (Exception exception)
      {
        await ctx.RespondAsync(exception.Message);
      }

      await ctx.RespondAsync("Command deleted");
    }

    /*
    * Method to use CustomCommand
    */
    [Command("ccUse"), Description("Command to use your CustomCommand")]
    public async Task RespondToCommand(CommandContext ctx, string commandName)
    {
      try
      {
        CustomCommandTable ccTable = new CustomCommandTable();

        var dbEntry = Database.runSQL($"Select * FROM CustomCommands WHERE CommandName = '{commandName}' LIMIT 1");

        if (dbEntry.Count != 0)
        {
          ccTable.CustomCommandId = Convert.ToInt32(dbEntry[0].GetValue(0));
          ccTable.CommandName = Convert.ToString(dbEntry[0].GetValue(1));
          ccTable.CommandResponse = Convert.ToString(dbEntry[0].GetValue(2));
          ccTable.DateCreated = Convert.ToDateTime(dbEntry[0].GetValue(3));
          ccTable.CreatedBy = Convert.ToString(dbEntry[0].GetValue(4));

          // Problem with DBNull
          if (dbEntry[0].GetValue(5) == System.DBNull.Value)
          {
            ccTable.ModifiedBy = "No Changes";
          }
          else
          {
            ccTable.ModifiedBy = Convert.ToString(dbEntry[0].GetValue(5));
          }

          //Problem with DateTime = Empty Not null/MinValue
          if (dbEntry[0].GetValue(6) == System.DBNull.Value)
          {
            ccTable.DateModified = DateTime.MinValue;
          }
          else
          {
            ccTable.DateModified = Convert.ToDateTime(dbEntry[0].GetValue(6));
          }
        }
        else
        {
          ccTable.CommandResponse = "No Commands found. Maybe the wrong Title?";
        }

        await ctx.RespondAsync(ccTable.CommandResponse);
      }
      catch (Exception ex)
      {
        await ctx.RespondAsync(ex.Message);
      }
    }

    /*
    * Method to get CustomCommand List
    */
    [Command("ccList"), Description("Command to get CustomCommandList")]
    public async Task CommandList(CommandContext ctx)
    {
      try
      {
        var dbEntry = Database.runSQL($"Select * FROM CustomCommands");

        string response = String.Empty;

        if (dbEntry.Count != 0)
        {
          foreach (var item in dbEntry)
          {
            response = response + "\n```\n" + item.GetValue(1) + "\n" + item.GetValue(2) + "```";
          }
        }
        else
        {
          response = "No Commands found";
        }

        await ctx.RespondAsync(response);
      }
      catch (Exception ex)
      {
        await ctx.RespondAsync(ex.Message);
      }
    }
  }
}
