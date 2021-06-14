using DiscordBot.DB;
using DiscordBot.Model.DbTables;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot
{
  class CustomCommands
  {
    #region Variables
    string commandName = String.Empty;
    string commandResponse = String.Empty;
    string user = String.Empty;
    string addToDb = String.Empty;
    #endregion

    //Statement um Tabelle zu erstellen
    public string CreateCustomCommandsTable()
    {
      string statement = "CREATE TABLE 'customcommands' ("
                         + "'CustomCommandId' int(11) NOT NULL AUTO_INCREMENT,"
                         + "'CommandName' varchar(100) CHARACTER SET utf8 DEFAULT NULL,"
                         + "'CommandResponse' varchar(2000) CHARACTER SET utf8 DEFAULT NULL,"
                         + "'DateCreated' datetime DEFAULT NULL,"
                         + "'CreatedBy' varchar(100) CHARACTER SET utf8 DEFAULT NULL,"
                         + "'ModifiedBy' varchar(100) CHARACTER SET utf8 DEFAULT NULL,"
                         + "'DateModified' datetime DEFAULT NULL,"
                         + "PRIMARY KEY ('CustomCommandId')"
                         + ");";

      return statement;
    }

    //Methode zum Hinzufügen eines TextCommands
    public string AddDatabaseEntry(MessageCreateEventArgs message)
    {
      try
      {
        commandName = message.Message.Content.Split(" ")[1].ToString();
        commandResponse = message.Message.Content.Split(new[] { ' ' }, 3)[2].ToString();

        user = message.Message.Author.ToString();

        addToDb = $"INSERT INTO customcommands(CommandName, CommandResponse, DateCreated, CreatedBy) VALUES('{commandName}', '{commandResponse}', NOW(), '{user}')";

        Database.runSQL(addToDb);
      }
      catch (Exception exception)
      {
        return exception.Message;
      }

      return "Command hinzugefügt.";
    }

    //Methode zum Ändern des CommandResponse
    public string UpdateDatabaseEntry(MessageCreateEventArgs message)
    {
      try
      {
        commandName = message.Message.Content.Split(" ")[1].ToString();
        commandResponse = message.Message.Content.Split(new[] { ' ' }, 3)[2].ToString();

        user = message.Message.Author.ToString();

        addToDb = $"UPDATE customcommands SET CommandName = '{commandName}', CommandResponse = '{commandResponse}', DateModified = Now(), ModifiedBy = '{user}' WHERE CommandName = '{commandName}'";

        Database.runSQL(addToDb);
      }
      catch (Exception exception)
      {
        return exception.Message;
      }

      return "Command geändert.";
    }

    //Methode zum Löschen eines Commands aufgrund es Titels
    public string DeleteDatabaseEntry(MessageCreateEventArgs message)
    {
      try
      {
        commandName = message.Message.Content.Split(" ")[1].ToString();

        addToDb = $"DELETE FROM customcommands WHERE CommandName = '{commandName}'";

        Database.runSQL(addToDb);
      }
      catch (Exception exception)
      {
        return exception.Message;
      }

      return "Command gelöscht.";
    }

    //Methode um auf einen Command zu reagieren
      public string RespondToCommand(MessageCreateEventArgs message)
      {
        try
        {
          CustomCommandTable ccTable = new CustomCommandTable();
          string title = message.Message.Content.Split("!")[1];

          var dbEntry = Database.runSQL($"Select * FROM CustomCommands WHERE CommandName = '{title}' LIMIT 1");

          ccTable.CustomCommandId = Convert.ToInt32(dbEntry[0].GetValue(0));
          ccTable.CommandName = Convert.ToString(dbEntry[0].GetValue(1));
          ccTable.CommandResponse = Convert.ToString(dbEntry[0].GetValue(2));
          ccTable.DateCreated = Convert.ToDateTime(dbEntry[0].GetValue(3));
          ccTable.CreatedBy = Convert.ToString(dbEntry[0].GetValue(4));

          // Problem with DBNull
          if (dbEntry[0].GetValue(5) == System.DBNull.Value)
          {
            ccTable.ModifiedBy = "Keine Änderungen";
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

          return ccTable.CommandResponse;
        }
        catch (Exception ex)
        {
          return ex.Message;
        }
      }
  }
}
