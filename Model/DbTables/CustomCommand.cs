using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Model.DbTables
{
  class CustomCommand
  {
    public int CustomCommandId { get; set; }

    public string CommandName { get; set; }

    public string CommandResponse { get; set; }

    public DateTime DateCreated { get; set; }

    public string CreatedBy { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime DateModified { get; set; }
  }
}
