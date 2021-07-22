using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;

namespace DiscordBot.STFU
{
  class MuteCommandTable
  {
    public DiscordChannel[] arrayChannels { get; set; }
    public DiscordMember[] arrayMembers { get; set; }
  }
}
