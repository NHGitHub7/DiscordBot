using DiscordBot.DB;
using DiscordBot.Model.DbTables;
using DSharpPlus.EventArgs;
using System;
using DiscordBot.Swearwords;
using System.Collections.Generic;
using System.Text;
using DiscordBot.Rolemanager;
using DSharpPlus;

namespace DiscordBot.Helper
{
  class DcMessageDistributor
  {
    //Methode die eine Nachricht passend weiterleitet
    public string GetMessage(MessageCreateEventArgs message)
    {
      // CustomCommand customCommand = new CustomCommand();
      string response = String.Empty;
      Blacklist bl = new Blacklist();

      // Zuweisung für CustomCommands
      #region CustomCommands
      CustomCommands customCommand = new CustomCommands();
      string msg = message.Message.Content.ToLower();

      if (msg.StartsWith("!addcustomcommand"))
      {
        response = customCommand.AddDatabaseEntry(message);
      }
      else if (msg.StartsWith("!updatecustomcommand"))
      {
        response = customCommand.UpdateDatabaseEntry(message);
      }
      else if (msg.StartsWith("!deletecustomcommand"))
      {
        response = customCommand.DeleteDatabaseEntry(message);
      }
      else if (msg.StartsWith("!"))
      {
        response = customCommand.RespondToCommand(message);
      }
      #endregion

      return response;
    }
  }
}
