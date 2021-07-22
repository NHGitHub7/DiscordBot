using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;

namespace DiscordBot.Rolemanager
{
  class RoleCommandTable
  {
    public DiscordChannel[] arrayChannels { get; set; }
    public DiscordGuild[] arrayGuilds { get; set; }
    public DiscordRole[] arrayRoles { get; set; }

  }
}
