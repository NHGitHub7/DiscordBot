using DiscordBot.Helper;
using DiscordBot.DB;
using DSharpPlus;
using System.Threading.Tasks;
using System;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext.Attributes;
using System.Text.Json;
using System.Text.Json.Serialization;
using DiscordBot.Rolemanager;
using System.Collections.Specialized;
using System.Threading.Channels;
using Newtonsoft.Json.Linq;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
namespace DiscordBot
{
  public class MuteCommand
  {
    //Prüfung der Berechtigung und Cooldown für den Befehl.
    [Command("stfu"), RequireUserPermissions(Permissions.Administrator), Cooldown(1, 30, CooldownBucketType.Channel)]

    public async Task STFU(CommandContext ctx)
    {

      //Was passieren sollte: Alle User in dem Sprachkanal des ausführenden Users, sollen gemuted werden.
      //                      Ausgenommen davon ist der ausführende User.
      //                      Nach ablauf eines weiteren Cooldowns werden die User wieder entmuted.
      await ctx.RespondAsync("Silence!");
    }


    //Meldung falls die Prüfung fehlschlägt.
    public ChecksFailedException(Command STFU, CommandContext ctx, IEnumerable<Permissions> failedChecks)
    {
      await ctx.RespondAsync("Access dinied!");
    }
  }
}
