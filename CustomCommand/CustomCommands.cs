using DiscordBot.DB;
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
    Database database = new Database();

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

        database.runSQL(addToDb);
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

        database.runSQL(addToDb);
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

        database.runSQL(addToDb);
      }
      catch (Exception exception)
      {
        return exception.Message;
      }

      return "Command gelöscht.";
    }

    //Methode um auf einen Command zu reagieren
    public string RespondToCommand()
    {
      string response = String.Empty;
      object[] dbEntry;

      dbEntry = database.runSQL("");
      

      return response;
    }
  }
}
