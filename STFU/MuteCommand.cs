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
    [Command("stfu"), RequirePermissions(Permissions.Administrator), Cooldown(1, 5, CooldownBucketType.Channel)]
    public async Task STFU(CommandContext ctx)
    {
      MuteCommandTable mcTable = new MuteCommandTable();
      string muted = String.Empty;
      mcTable.arrayChannels = new List<DiscordChannel>(ctx.Guild.Channels.Values).ToArray();
      foreach (var arrayChannel in mcTable.arrayChannels)
      {
        if (arrayChannel.Type == ChannelType.Voice)
        {
          mcTable.arrayMembers = new List<DiscordMember>(arrayChannel.Users).ToArray();
          foreach (var arrayMember in mcTable.arrayMembers)
          {

            if (arrayMember.Id != ctx.Member.Id && arrayMember.IsMuted == false)
            {
              await arrayMember.SetMuteAsync(true);
              muted = "Usersmute";
            }
            else if (arrayMember.IsMuted)
            {
              await arrayMember.SetMuteAsync(false);
              muted = "Usersunmute";
            }
            else if (arrayMember.Id == ctx.Member.Id)
            {
              await arrayMember.SetMuteAsync(false);
            }
          }
        }
      }
      if (muted == "Usersmute")
      {
        await ctx.RespondAsync("Silence!");
      }
      else if (muted == "Usersunmute")
      {
        await ctx.RespondAsync("You can now speak again!");
      }
    }
  }
}

