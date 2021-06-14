using DiscordBot.DB;
using DiscordBot.Model.DbTables;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Helper
{
  class DcMessageDistributor
  {
    //Methode die eine Nachricht passend weiterleitet
    public string GetMessage(MessageCreateEventArgs message)
    {
      // CustomCommand customCommand = new CustomCommand();
      string response = String.Empty;

      // Zuweisung für CustomCommands
      #region CustomCommands
      CustomCommands customCommand = new CustomCommands();

      if (message.Message.Content.ToLower().StartsWith("!addcustomcommand"))
      {
        response = customCommand.AddDatabaseEntry(message);
      }
      else if (message.Message.Content.ToLower().StartsWith("!updatecustomcommand"))
      {
        response = customCommand.UpdateDatabaseEntry(message);
      }
      else if (message.Message.Content.ToLower().StartsWith("!deletecustomcommand"))
      {
        response = customCommand.DeleteDatabaseEntry(message);
      }
      else if (message.Message.Content.ToLower().StartsWith("!"))
      {
        response = customCommand.RespondToCommand(message);
      }
      #endregion

      return response;
    }
  }
}
