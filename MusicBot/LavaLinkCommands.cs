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
  }
}
