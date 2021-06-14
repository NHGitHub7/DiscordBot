using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DiscordBot.DB;
using DSharpPlus.Entities;

namespace DiscordBot.Rolemanager
{
  class RoleEventReactions
  {
    public async Task ReactOnUserJoin(DiscordClient s, GuildMemberAddEventArgs e)
    {
      await e.Member.CreateDmChannelAsync();
      await e.Member.SendMessageAsync("Do you have any special Keycode?");
    }

    public async Task ReactOnUserMessage(DiscordClient s, MessageCreateEventArgs e, DiscordClient discord)
    {
      DiscordChannel[] arrayChannels;
      DiscordGuild[] arrayGuilds;
      DiscordRole[] arrayRoles;
      var tmp = Database.runSQL("SELECT rolename, keycode FROM CustomRoles");
      
      /*
       * Bot reacts to private User Message to get his defined Role.
       * SQL Query template used for tests(Roles need to exist on Discord Server):
       * create table CustomRoles
        (
          RoleID int primary key auto_increment,
          rolename varchar(255),
          keycode varchar(255)
         );
          insert into CustomRoles (rolename, keycode)
          values("Moderator", "Moderator123");
          insert into CustomRoles (rolename, keycode)
          values ("Supporter", "Supporter123");
          insert into CustomRoles (rolename, keycode)
          values ("Member", "Member123");
       */
      for (var i = 0; i < tmp.Count; i++)
      {
        for (var j = 0; j < tmp[i].Length; j++)
        {
          if (e.Channel.IsPrivate == true && e.Message.Content.StartsWith(tmp[i][1].ToString()))
          {
            arrayGuilds = new List<DiscordGuild>(discord.Guilds.Values).ToArray();
            foreach (var guild in arrayGuilds)
            {
              arrayRoles = new List<DiscordRole>(guild.Roles.Values).ToArray();

              foreach (var role in arrayRoles)
              {
                if (role.Name == tmp[i][0].ToString())
                  await guild.Members[e.Author.Id].GrantRoleAsync(role);
              }
            }
          }
        }
      }
    }
  }
}
