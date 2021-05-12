using DiscordBot.Helper;
using DSharpPlus;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args)
        {

            sql_conn();
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            ConfigurationHelper configurationHelper = new ConfigurationHelper();

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = configurationHelper.GetOAuthValue().Token,
                TokenType = TokenType.Bot
            });

            discord.MessageCreated += async (s, e) =>
            {
                if (e.Message.Content.ToLower().StartsWith("ping"))
                    await e.Message.RespondAsync("pong!");
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        static void sql_conn()
        {
            System.Text.StringBuilder connString = new System.Text.StringBuilder();
            connString.Append("Server=localhost;")
                .Append("Port=3308;")
                .Append("Database=discord_bot;")
                .Append("Uid=root;")
                .Append("password=Admin_01!;");

            Console.WriteLine(connString.ToString());
            using var con = new MySqlConnection(connString.ToString());
            con.Open();
            
            var stm = "SELECT VERSION()";
            var cmd = new MySqlCommand(stm, con);

            var version = cmd.ExecuteScalar().ToString();
            Console.WriteLine($"MySQL version: {version}");
        }
    }
}
