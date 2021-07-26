using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Lavalink;
using System.Linq;
using System;

namespace DiscordBot.MusicBot
{
  public class LavaLinkCommands : BaseCommandModule
  {
    private LavalinkNodeConnection Lavalink { get; set; }
    private LavalinkGuildConnection LavalinkVoice { get; set; }
    private DiscordChannel ContextChannel { get; set; }

    /*
     * Method to join bot into Channel
     */
    [Command("join"), Description("Command to join the Bot into Voice-Channel")]
    public async Task JoinChannel(CommandContext ctx, DiscordChannel channel)
    {
      var lava = ctx.Client.GetLavalink();
      if (!lava.ConnectedNodes.Any())
      {
        await ctx.RespondAsync("No Connection to LavaLink");
        return;
      }

      var node = lava.ConnectedNodes.Values.First();

      if (channel.Type != ChannelType.Voice)
      {
        await ctx.RespondAsync("Not in a valid Voice-Channel");
        return;
      }

      await node.ConnectAsync(channel);
      await ctx.RespondAsync($"Joined {channel.Name}!");
    }

    /*
     * Method to kick bot out of Channel
     */
    [Command("leave"), Description("Command to let the Bot leave Voice-Channel")]
    public async Task LeaveChannel(CommandContext ctx, DiscordChannel channel)
    {
      var lava = ctx.Client.GetLavalink();
      if (!lava.ConnectedNodes.Any())
      {
        await ctx.RespondAsync("No Connection to LavaLink");
        return;
      }

      var node = lava.ConnectedNodes.Values.First();

      if (channel.Type != ChannelType.Voice)
      {
        await ctx.RespondAsync("Not in a valid Voice-Channel");
        return;
      }

      var conn = node.GetGuildConnection(channel.Guild);

      if (conn == null)
      {
        await ctx.RespondAsync("LavaLink not Connected");
        return;
      }

      await conn.DisconnectAsync();
      await ctx.RespondAsync($"Left {channel.Name}!");
    }

    /*
     * Method to play Music
     */
    [Command("play"), Description("Command to play Music")]
    public async Task Play(CommandContext ctx, [RemainingText] string search)
    {
      if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
      {
        await ctx.RespondAsync("You are not in the same Voice-Channel");
        return;
      }

      var lava = ctx.Client.GetLavalink();
      var node = lava.ConnectedNodes.Values.First();
      var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

      if (conn == null)
      {
        await ctx.RespondAsync("LavaLink not Connected");
        return;
      }

      var loadResult = await node.Rest.GetTracksAsync(search);

      if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
          || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
      {
        await ctx.RespondAsync($"Song Search failed for `{search}`.");
        return;
      }

      var track = loadResult.Tracks.First();

      await conn.PlayAsync(track);

      await ctx.RespondAsync($"Now playing: `{track.Title}`!");
    }

    /*
     * Method to stop Music
     */
    [Command("stop"), Description("Command to stop Music")]
    public async Task Pause(CommandContext ctx)
    {
      if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
      {
        await ctx.RespondAsync("Not in a valid Voice-Channel");
        return;
      }

      var lava = ctx.Client.GetLavalink();
      var node = lava.ConnectedNodes.Values.First();
      var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

      if (conn == null)
      {
        await ctx.RespondAsync("Lavalink not Connected");
        return;
      }

      if (conn.CurrentState.CurrentTrack == null)
      {
        await ctx.RespondAsync("No Songs loaded");
        return;
      }

      await conn.PauseAsync();
    }

    /*
     * Method to pause Music
     */
    [Command("pause"), Description("Command to pause Music")]
    public async Task PauseAsync(CommandContext ctx)
    {
      if (this.LavalinkVoice == null)
        return;

      await this.LavalinkVoice.PauseAsync().ConfigureAwait(false);
      await ctx.RespondAsync("Paused playing").ConfigureAwait(false);
    }

    /*
     * Method to resume Music
     */
    [Command("resume"), Description("Command to resume Music")]
    public async Task ResumeAsync(CommandContext ctx)
    {
      if (this.LavalinkVoice == null)
        return;

      await this.LavalinkVoice.ResumeAsync().ConfigureAwait(false);
      await ctx.RespondAsync("Resumed playing").ConfigureAwait(false);
    }

    /*
     * Method to play TimeSpan from Song
     */
    [Command("playpart"), Description("Command to play TimeSpan from Song")]
    public async Task PlayPartialAsync(CommandContext ctx, TimeSpan start, TimeSpan stop, [RemainingText] Uri uri)
    {
      if (this.LavalinkVoice == null)
        return;

      var trackLoad = await this.Lavalink.Rest.GetTracksAsync(uri).ConfigureAwait(false);
      var track = trackLoad.Tracks.First();
      await this.LavalinkVoice.PlayPartialAsync(track, start, stop).ConfigureAwait(false);

      await ctx.RespondAsync($"Now playing: {Formatter.Bold(Formatter.Sanitize(track.Title))} by {Formatter.Bold(Formatter.Sanitize(track.Author))}").ConfigureAwait(false);
    }

    /*
     * Method to change Volume
     */
    [Command("volume"), Description("Command to change Volume")]
    public async Task VolumeAsync(CommandContext ctx, int volume)
    {
      if (this.LavalinkVoice == null)
        return;

      await this.LavalinkVoice.SetVolumeAsync(volume).ConfigureAwait(false);
      await ctx.RespondAsync($"Volume set to {volume}%").ConfigureAwait(false);
    }

    /*
     * Method to get current Song
     */
    [Command("nowplaying"), Description("Command to show current Song")]
    public async Task NowPlayingAsync(CommandContext ctx)
    {
      if (this.LavalinkVoice == null)
        return;

      var state = this.LavalinkVoice.CurrentState;
      var track = state.CurrentTrack;
      await ctx.RespondAsync($"Now playing: {Formatter.Bold(Formatter.Sanitize(track.Title))} by {Formatter.Bold(Formatter.Sanitize(track.Author))} [{state.PlaybackPosition}/{track.Length}].").ConfigureAwait(false);
    }
  }
}
