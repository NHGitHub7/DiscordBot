using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus;
using System.Threading.Tasks;
using DiscordBot.DB;

namespace DiscordBot.Rolemanager
{
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
            var returnsql = db.runSQL(sqlQuery);
            if (returnsql.Count != 0 )
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
