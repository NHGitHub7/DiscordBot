using DiscordBot.Helper;
using DiscordBot.DB;
using DSharpPlus;
using System.Threading.Tasks;
using System;
using System.Reflection.Metadata.Ecma335;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using DSharpPlus.CommandsNext;
using MySqlX.XDevAPI;
using Client = MySql.Data.MySqlClient.Memcached.Client;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Text.Json;
using System.Text.Json.Serialization;
using DiscordBot.Rolemanager;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;


namespace DiscordBot
{
  class Program
  {
    static void Main(string[] args)
    {
            //MainAsync().GetAwaiter().GetResult();
            //GuildmemberTask().GetAwaiter().GetResult();
            var db = new Database();

            string sqlQuery = "SELECT rolename, keycode FROM CustomRoles";
            var returnvalue = db.runSQL(sqlQuery);
            Console.WriteLine(returnvalue);
    }
    static async Task MainAsync()
    {
      ConfigurationHelper configurationHelper = new ConfigurationHelper();

      var discord = new DiscordClient(new DiscordConfiguration()
      {
        Token = configurationHelper.GetOAuthValue().Token,
        TokenType = TokenType.Bot
      });

      DiscordChannel[] arrayChannels;
      DiscordGuild[] arrayGuilds;
      DiscordRole[] arrayRoles;


      discord.MessageCreated += async (s, e) =>
        {

            if (e.Message.Content.ToLower().StartsWith("ping"))
                await e.Message.RespondAsync("pong");
            if (e.Message.Content.ToLower().StartsWith("!getdiscord"))
            {

                arrayGuilds = new List<DiscordGuild>(discord.Guilds.Values).ToArray();
                foreach (var guild in arrayGuilds)
                {

                    //arrayChannels = new List<DiscordChannel>(guild.Channels.Values).ToArray();

                    //foreach (var channel in arrayChannels)
                    //{

                    //    await e.Message.RespondAsync(channel.Name);
                    //}
                }
            }
        };

      await discord.ConnectAsync();
      await Task.Delay(-1);

    }

    static async Task GuildmemberTask()
    {
        ConfigurationHelper configurationHelper = new ConfigurationHelper();

        var discord = new DiscordClient(new DiscordConfiguration()
        {
            Token = configurationHelper.GetOAuthValue().Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All
        });
        DiscordChannel[] arrayChannels;
        DiscordGuild[] arrayGuilds;
        DiscordRole[] arrayRoles;
        GuildMemberAddEventArgs globalMember = null;
        /*
         * Command Prefix you need, to use the Command.
         */
        var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
        {
            StringPrefixes = new[] {"!"}
        });
        commands.RegisterCommands<RoleCommands>();
        /*
         * Event that reacts on User Join in your Guild.
         */
        discord.GuildMemberAdded += async (s, e) =>
        {
            await e.Member.CreateDmChannelAsync();
            await e.Member.SendMessageAsync("Do you have any special Keycode?");
            globalMember = e;
        };
        /*
         * Event that reacts on Message written from User.
         */
        discord.MessageCreated += async (s, e) =>
        {
            /*
             * Bot reacts to private User Message to get his defined Role.
             */
            if (e.Channel.IsPrivate == true && e.Message.Content.ToLower().StartsWith("givemerole"))
            {
                arrayGuilds = new List<DiscordGuild>(discord.Guilds.Values).ToArray();
                foreach (var guild in arrayGuilds)
                {
                    arrayRoles = new List<DiscordRole>(guild.Roles.Values).ToArray();

                    foreach (var role in arrayRoles)
                    {
                        if (role.Name == "TestF")

                            await globalMember.Member.GrantRoleAsync(role);
                    }
                }
            }
        };
        await discord.ConnectAsync();
        await Task.Delay(-1);
        }
  }

  public class RoleCommands : BaseCommandModule
  { 
      
      [Command("saverolewithpw")]
      public async Task AddKeyCodeToRole(CommandContext ctx, string rolename, string keycode)
      {
          await SaveRoleToDB(rolename, keycode, ctx);
      }
      [Command("updaterolewithpw")]
      public async Task UpdateKeyCodeFromRole(CommandContext ctx, string rolename, string keycode)
      {
          await UpdateRoleInDB(rolename, keycode, ctx);
      }

      private async Task SaveRoleToDB(string customrolename, string customkeycode, CommandContext ctx)
      {
          var db = new Database();
          //string sqlQuery = $"INSERT INTO CustomRoles (rolename, keycode) VALUES ('{customrolename}', '{customkeycode}')";
          string sqlQuery = $"SELECT * FROM CustomRoles WHERE rolename='{customrolename}'";
          string returnsql = db.runSQL(sqlQuery);
          if (returnsql == "1")
          {
              await ctx.RespondAsync("This Role already exists with a Password. If you want to update the Password use !updaterolewithpw {rolename} {keycode}");
          }
          else
          {
              sqlQuery = $"INSERT INTO CustomRoles (rolename, keycode) VALUES ('{customrolename}', '{customkeycode}')";
              db.runSQL(sqlQuery);
          }
      }

      private async Task UpdateRoleInDB(string customrolename, string customkeycode, CommandContext ctx)
      {
          var db = new Database();

          string sqlQuery = $"UPDATE CustomRoles SET keycode = '{customkeycode}' WHERE rolename='{customrolename}'";
          db.runSQL(sqlQuery);
      }
    }
}
