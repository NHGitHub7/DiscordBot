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
    //Prüfung der Berechtigung und Cooldown für den Befehl.
    [Command("stfu"), RequireUserPermissions(Permissions.Administrator),
    Cooldown(1, 30, CooldownBucketType.Channel)]
    public async Task STFU(CommandContext ctx)
    {
      MuteCommandTable mcTable = new MuteCommandTable();
      //Was passieren sollte: Alle User in dem Sprachkanal des ausführenden Users, sollen gemuted werden.
      //                    Ausgenommen davon ist der ausführende User.
      //                  Nach ablauf eines weiteren Cooldowns werden die User wieder entmuted.
      await ctx.RespondAsync("Silence!");
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
 
