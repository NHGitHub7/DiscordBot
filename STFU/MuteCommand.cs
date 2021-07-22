using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus;
using System.Threading.Tasks;
using DiscordBot.DB;
using System.Reflection;
using System.Runtime.InteropServices;
using DSharpPlus.Entities;
using DiscordBot.STFU;

namespace DiscordBot.STFU
{

  public class MuteCommand : BaseCommandModule
  {
    /*
     * Mutes all other Users in your current Voice Channel.
     */
    [Command("stfu"), RequireUserPermissions(Permissions.Administrator)]
    public async Task STFU(CommandContext ctx)
    {
      MuteCommandTable mcTable = new MuteCommandTable();
      mcTable.arrayChannels = new List<DiscordChannel>(ctx.Guild.Channels.Values).ToArray();
      foreach (var arrayChannel in mcTable.arrayChannels)
      {
        mcTable.arrayMembers = new List<DiscordMember>(arrayChannel.Users).ToArray();
        foreach (var arrayMember in mcTable.arrayMembers)
        {
          if (arrayMember.Id != ctx.Member.Id && arrayMember.IsMuted == false)
          {
            await arrayMember.SetMuteAsync(true);
            await ctx.RespondAsync("Silence!");
          }
          else if (arrayMember.IsMuted)
          {
            await arrayMember.SetMuteAsync(false);
            await ctx.RespondAsync("You now can speak again!!");
          }
          else
          {
            await arrayMember.SetMuteAsync(false);
          }
        }
      }
    }
  }
}

