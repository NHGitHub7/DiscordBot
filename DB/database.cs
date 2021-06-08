using MySql.Data.MySqlClient;
using DiscordBot.Helper;
using System;
using System.IO;

namespace DiscordBot.DB
{
  public class Database
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

      this.conn = connection;
    }
    public string runSQL(string query)
    {

      this.conn.Open();

      var cmd = new MySqlCommand(query, this.conn);

      string return_val = cmd.ExecuteReader().ToString();

      this.conn.Close();

      return return_val;
    }

    public void defaultSetup()
    {
      if (!this.tablesExist())
      {
        Console.WriteLine("No version Table found.\nWriting DDL File to DB.\nPlease wait.");
        this.writeDefaultSetup();
        Console.WriteLine("DB Tables created");
      }
      else
      {
        Console.WriteLine("DB is up to date");
      }
    }
    bool tablesExist()
    {
      this.conn.Open();

      string query =
        "SELECT CREATE_TIME " +
        "FROM information_schema.tables " +
        "WHERE table_schema = 'discord_bot' " +
        "AND table_name = 'version' " +
        "LIMIT 1";

      var cmd = new MySqlCommand(query, this.conn);
      object check = cmd.ExecuteScalar();
      conn.Close();

      if (check == null)
      {
        return false;
      }
      else
      {
        return true;
      }
    }

    void writeDefaultSetup()
    {
      this.conn.Open();
      string ddl = File.ReadAllText(@"DB\DDL.sql");
      var cmd = new MySqlCommand(ddl, this.conn);
      cmd.ExecuteScalar();
      this.conn.Close();
    }
  }
}