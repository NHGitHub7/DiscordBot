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
      string response = String.Empty;

      // Zuweisung für CustomCommands
      #region CustomCommands
      CustomCommands customCommand = new CustomCommands();

      if (message.Message.Content.ToLower().StartsWith("!"))
      {
        response = customCommand.RespondToCommand(message);
      }
      #endregion

      return response;
    }
  }
}
