using MySql.Data.MySqlClient;
using DiscordBot.Helper;

namespace DiscordBot.DB
{
  class Database
  {
    string host;
    int port;
    string name;
    string user;
    string password;

    public Database()
    {
      ConfigurationHelper configHelper = new ConfigurationHelper();
      DiscordBot.Model.DB_Access db = configHelper.GetDBAccessValues();
      host = db.host;
      port = db.port;
      name = db.name;
      user = db.user;
      password = db.password;
    }
    public string runSQL(string query)
    {
      System.Text.StringBuilder connString = new System.Text.StringBuilder();
      connString.Append($"Server={this.host};")
          .Append($"Port={this.port};")
          .Append($"Database={this.name};")
          .Append($"Uid={this.user};")
          .Append($"password={this.password};");

      using var conn = new MySqlConnection(connString.ToString());
      conn.Open();

      var cmd = new MySqlCommand(query, conn);

      return cmd.ExecuteScalar().ToString();
    }
  }
}