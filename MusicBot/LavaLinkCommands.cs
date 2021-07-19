using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Lavalink;
using System.Linq;

namespace DiscordBot.MusicBot
{
  public class LavaLinkCommands : BaseCommandModule
  {
    [Command("join")]
    public async Task JoinChannel(CommandContext ctx, DiscordChannel channel)
    {
      var lava = ctx.Client.GetLavalink();
      if (!lava.ConnectedNodes.Any())
      {
        await ctx.RespondAsync("Keine Konnektivität zu LavaLink hergestellt.");
        return;
      }

      var node = lava.ConnectedNodes.Values.First();

      if (channel.Type != ChannelType.Voice)
      {
        await ctx.RespondAsync("Nicht in einem gültigen Voice-Channel.");
        return;
      }

      await node.ConnectAsync(channel);
      await ctx.RespondAsync($"Joined {channel.Name}!");
    }

    [Command("leave")]
    public async Task LeaveChannel(CommandContext ctx, DiscordChannel channel)
    {
      var lava = ctx.Client.GetLavalink();
      if (!lava.ConnectedNodes.Any())
      {
        await ctx.RespondAsync("Keine Konnektivität zu LavaLink hergestellt.");
        return;
      }

      var node = lava.ConnectedNodes.Values.First();

      if (channel.Type != ChannelType.Voice)
      {
        await ctx.RespondAsync("Nicht in einem gültigen Voice-Channel.");
        return;
      }

      var conn = node.GetGuildConnection(channel.Guild);

      if (conn == null)
      {
        await ctx.RespondAsync("LavaLink nicht verbunden.");
        return;
      }

      await conn.DisconnectAsync();
      await ctx.RespondAsync($"Left {channel.Name}!");
    }

    [Command("play")]
    public async Task Play(CommandContext ctx, [RemainingText] string search)
    {
      if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
      {
        await ctx.RespondAsync("Du bist nicht in einem Voice-Channel.");
        return;
      }

      var lava = ctx.Client.GetLavalink();
      var node = lava.ConnectedNodes.Values.First();
      var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

      if (conn == null)
      {
        await ctx.RespondAsync("LavaLink nicht verbunden.");
        return;
      }

      var loadResult = await node.Rest.GetTracksAsync(search);

      if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
          || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
      {
        await ctx.RespondAsync($"Songsuche fehlgeschlagen für `{search}`.");
        return;
      }

      var track = loadResult.Tracks.First();

      await conn.PlayAsync(track);

      await ctx.RespondAsync($"Spielt jetzt: `{track.Title}`!");
    }

    [Command("pause")]
    public async Task Pause(CommandContext ctx)
    {
      if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
      {
        await ctx.RespondAsync("Du bist nicht in einem Voice-Channel.");
        return;
      }

      var lava = ctx.Client.GetLavalink();
      var node = lava.ConnectedNodes.Values.First();
      var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

      if (conn == null)
      {
        await ctx.RespondAsync("Lavalink nicht verbunden.");
        return;
      }

      if (conn.CurrentState.CurrentTrack == null)
      {
        await ctx.RespondAsync("Keine Songs geladen.");
        return;
      }

      await conn.PauseAsync();
    }
  }
}
