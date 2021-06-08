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
      string response = String.Empty;

      // Zuweisung für CustomCommands
      #region CustomCommands
      CustomCommand customCommand = new CustomCommand();

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
      #endregion

      else
      {
        //response = "Keine Übereinstimmung gefunden.";
      }

      return response;
    }
  }
}
