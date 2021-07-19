using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DiscordBot.DB;
using DSharpPlus.Entities;
using DiscordBot.Rolemanager;

namespace DiscordBot.Rolemanager
{
  class RoleEventReactions
  {
    public async Task ReactOnUserJoin(DiscordClient s, GuildMemberAddEventArgs e)
    {
      await e.Member.CreateDmChannelAsync();
      await e.Member.SendMessageAsync("Do you have any special Password?");
    }

    public async Task ReactOnUserMessage(MessageCreateEventArgs e, DiscordClient discord)
    {

      var tmp = Database.runSQL("SELECT rolename, password, active FROM customroles");
      RoleCommandTable rcTable = new RoleCommandTable();
      bool rolegranted = false;
      /*
       * Bot reacts to private User Message to get his defined Role.
       * SQL Query template used for tests(Roles need to exist on Discord Server):
       * create table customroles
        (
          RoleID int primary key auto_increment,
          rolename varchar(255),
          password varchar(255)
         );
          insert into customroles (rolename, password)
          values("Moderator", "Moderator123");
          insert into customroles (rolename, password)
          values ("Supporter", "Supporter123");
          insert into customroles (rolename, password)
          values ("Member", "Member123");
       */
      for (var i = 0; i < tmp.Count; i++)
      {
        for (var j = 0; j < tmp[i].Length; j++)
        {
          if (e.Channel.IsPrivate == true && e.Message.Content.StartsWith(tmp[i][1].ToString()) && Convert.ToBoolean(tmp[i][2]) == true)
          {
            rcTable.arrayGuilds = new List<DiscordGuild>(discord.Guilds.Values).ToArray();
            foreach (var guild in rcTable.arrayGuilds)
            {
              rcTable.arrayRoles = new List<DiscordRole>(guild.Roles.Values).ToArray();

              foreach (var role in rcTable.arrayRoles)
              {
                if (role.Name == tmp[i][0].ToString())
                {
                  await guild.Members[e.Author.Id].GrantRoleAsync(role);
                  rolegranted = true;
                }
              }
            }
          }
        }
      }
      if (e.Channel.IsPrivate == true && rolegranted == false)
      {
        await e.Message.RespondAsync("This Role does not exist, the password might be wrong, or this role is currently not active.");
      }
      else if (rolegranted)
      {
        await e.Message.RespondAsync("You will receive your Role.");
      }

    }

    public async Task SetRolesToDB(DiscordClient discord)
    {
      string sqlQuery = "DROP TABLE IF EXISTS customroles;" +
        "CREATE TABLE customroles(RoleID integer(11) NOT NULL AUTO_INCREMENT,rolename varchar(255),password varchar(255), active bool, PRIMARY KEY(RoleID));";
      Database.runSQL(sqlQuery);

      RoleCommandTable rcTable = new RoleCommandTable();
      rcTable.arrayGuilds = new List<DiscordGuild>(discord.Guilds.Values).ToArray();
      foreach (var guild in rcTable.arrayGuilds)
      {
        rcTable.arrayRoles = new List<DiscordRole>(guild.Roles.Values).ToArray();
        foreach(var role in rcTable.arrayRoles)
        {
          
            if (role.Name != "@everyone")
            {
              sqlQuery = $"INSERT INTO customroles(rolename, password, active) VALUES ('{role.Name}', 'default', false)";
              Database.runSQL(sqlQuery);
            }

        }
      }
    }
  }
}
