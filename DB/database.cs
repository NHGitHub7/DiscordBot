using MySql.Data.MySqlClient;
using DiscordBot.Helper;

namespace DiscordBot.DB
{
  class Database
  {
    MySqlConnection conn;

    public Database()
    {
      ConfigurationHelper configHelper = new ConfigurationHelper();
      DiscordBot.Model.DB_Access db = configHelper.GetDBAccessValues();

      System.Text.StringBuilder connString = new System.Text.StringBuilder();
      connString.Append($"Server={db.host};")
          .Append($"Port={db.port};")
          .Append($"Database={db.name};")
          .Append($"Uid={db.user};")
          .Append($"password={db.password};");

      using var connection = new MySqlConnection(connString.ToString());

      conn = connection;
    }

    //Methode um ein SQL Statement auszuführen
    public string runSQL(string query)
    {
      this.conn.Open();

      var cmd = new MySqlCommand(query, conn);

      string  return_val = (string)cmd.ExecuteScalar();

      conn.Close();

      return return_val;
    }
  }
}