using MySql.Data.MySqlClient;
using DiscordBot.Helper;
using System;
using System.IO;
using System.Collections.Generic;

namespace DiscordBot.DB
{
  static class Database
  {
    static MySqlConnection conn;

    public static void Init_Database()
    {
      ConfigurationHelper configHelper = new ConfigurationHelper();
      Model.DB_Access db = configHelper.GetDBAccessValues();

      System.Text.StringBuilder connString = new System.Text.StringBuilder();
      connString.Append($"Server={db.host};")
          .Append($"Port={db.port};")
          .Append($"Database={db.name};")
          .Append($"Uid={db.user};")
          .Append($"password={db.password};");

      MySqlConnection connection = new MySqlConnection(connString.ToString());

      conn = connection;
    }
    public static List<object[]> runSQL(string query)
    {

      conn.Open();

      var cmd = new MySqlCommand(query, conn);

      MySqlDataReader reader = cmd.ExecuteReader();

      var array_length = reader.FieldCount;
      List<object[]> tmp = new List<object[]>();

      while (reader.Read())
      {
        object[] values = new object[array_length];
        reader.GetValues(values);
        tmp.Add(values);
      }
      conn.Close();

      return tmp;
    }

    public static void defaultSetup()
    {
      if (!tablesExist())
      {
        Console.WriteLine("No version Table found.\nWriting DDL File to DB.\nPlease wait.");
        writeDefaultSetup();
        Console.WriteLine("DB Tables created");
      }
      else if (!versionUp2Date())
      {
        Console.WriteLine("Updated Version available. Rerunning DDL.");
        writeDefaultSetup();
        updateVersion();
        Console.WriteLine("Updated to new Version");
      }
      else
      {
        Console.WriteLine("DB is up to date");
      }
    }
    static bool tablesExist()
    {
      conn.Open();

      string query =
        "SELECT CREATE_TIME " +
        "FROM information_schema.tables " +
        "WHERE table_schema = 'discord_bot' " +
        "AND table_name = 'version' " +
        "LIMIT 1";

      var cmd = new MySqlCommand(query, conn);
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

    static bool versionUp2Date()
    {
      ConfigurationHelper configHelper = new ConfigurationHelper();
      Model.Versioning version_config = configHelper.GetVersion();
      string query =
        "SELECT version " +
        "FROM version " +
        "WHERE name like 'discord_bot'";
      conn.Open();
      var cmd = new MySqlCommand(query, conn);
      string version_db_str = cmd.ExecuteScalar().ToString();
      conn.Close();
      int version_db = Int32.Parse(version_db_str);

      return version_db == version_config.discord_bot;
    }

    static void updateVersion()
    {
      ConfigurationHelper configHelper = new ConfigurationHelper();
      Model.Versioning version_config = configHelper.GetVersion();
      if (version_config != null)
      {
        string query =
          "UPDATE version " +
          $"SET version = {version_config.discord_bot} " +
          "WHERE name like 'discord_bot'";
          conn.Open();
          var cmd = new MySqlCommand(query, conn);
          cmd.ExecuteScalar();
          conn.Close();
      } else
      {
        throw new ArgumentException("Parameter discord_bot cannot be null");
      }
    }

    static void writeDefaultSetup()
    {
      conn.Open();
      string ddl = File.ReadAllText(@"DB\DDL.sql");
      var cmd = new MySqlCommand(ddl, conn);
      cmd.ExecuteScalar();
      conn.Close();
    }
  }
}