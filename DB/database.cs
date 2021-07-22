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

    /**
     * Initialize the Database connection pool
     * This needs to be called at the beginning of the Runtime
     */
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

    /**
     * Generic function for running Querys
     */
    public static List<object[]> runSQL(string query)
    {

      conn.Open();

      MySqlCommand cmd = new MySqlCommand(query, conn);

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

    /**
     * Generic function for Querys that return nothing (Inserts) or one value.
     */
    public static object runScalar(string query)
    {
      MySqlCommand cmd = new MySqlCommand(query, conn);
      conn.Open();
      object return_val = cmd.ExecuteScalar();
      conn.Close();

      return return_val;
    }

    /**
     * Get all currently registered swearwords
     */
    public static List<string> get_swearwords()
    {
      try
      {
        List<string> swearwords = new List<string>();

        string query = "SELECT word " +
        "FROM swearwords";

        var cmd = new MySqlCommand(query, conn);
        conn.Open();
        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          swearwords.Add(reader.GetString(0));
        }
        conn.Close();

        return swearwords;
      }
      catch
      {
        throw new InvalidOperationException("Error reading from `swearwords` table");
      }
    }

    // @TODO: Move this back to Swearwords class
    public static object get_strikes_from_user(UInt64 user_id)
    {
      try
      {
        string query = "SELECT strikes " +
          "FROM swearword_strikes " +
          $"WHERE user_id = {user_id}";
        MySqlCommand cmd = new MySqlCommand(query, conn);
        conn.Open();
        object strikes = cmd.ExecuteScalar();
        conn.Close();
        return strikes;
      }
      catch
      {
        throw new InvalidOperationException("Error getting strikes of a user");
      }
    }

    /**
     * If we need to update/setup our DB do this here
     */
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
        // With more time this sould be rewritten to use a second File that only alters the tables.
        // With this all data currently written to the DB is lost.
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

    /**
     * Check if the version table exists in our database.
     */
    static bool tablesExist()
    {
      string query =
        "SELECT CREATE_TIME " +
        "FROM information_schema.tables " +
        "WHERE table_schema = 'discord_bot' " +
        "AND table_name = 'version' " +
        "LIMIT 1";

      var cmd = new MySqlCommand(query, conn);

      try
      {
        conn.Open();
      }
      catch (MySqlException e)
      {
        Console.WriteLine($"Error: {e}\n\n");
        Console.WriteLine("Check if your DB is available\n" +
          "Is your Docker Container running ?\n" +
          "Or Are the Values in your config.json correct ?");
        System.Environment.Exit(1);
      }

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

    /**
     * Get the Version from the db and compare it with the version in our config.json
     */
    static bool versionUp2Date()
    {
      try
      {
        ConfigurationHelper configHelper = new ConfigurationHelper();
        Model.Versioning version_config = configHelper.GetVersion();
        string query =
          "SELECT version " +
          "FROM version " +
          "WHERE name like 'discord_bot'";
        var cmd = new MySqlCommand(query, conn);
        conn.Open();
        string version_db_str = cmd.ExecuteScalar().ToString();
        conn.Close();
        int version_db = Int32.Parse(version_db_str);

        return version_db == version_config.discord_bot;
      }
      catch
      {
        throw new InvalidOperationException("Error getting version from the Database");
      }
    }

    /**
     * Persist the new version in the database.
     */
    static void updateVersion()
    {
      ConfigurationHelper configHelper = new ConfigurationHelper();
      Model.Versioning version_config = configHelper.GetVersion();
      if (version_config != null)
      {
        try
        {
          string query =
            "UPDATE version " +
            $"SET version = {version_config.discord_bot} " +
            "WHERE name like 'discord_bot'";
          conn.Open();
          var cmd = new MySqlCommand(query, conn);
          cmd.ExecuteScalar();
          conn.Close();
        }
        catch
        {
          throw new ArgumentException("Could not connect to the DB and update version");
        }
      }
      else
      {
        throw new ArgumentException("Version parameter discord_bot cannot be null");
      }
    }

    /**
     * Write everything in DDL.sql to the Database
     */
    static void writeDefaultSetup()
    {
      try
      {
        string ddl = File.ReadAllText(@"DB\DDL.sql");
        var cmd = new MySqlCommand(ddl, conn);
        conn.Open();
        cmd.ExecuteScalar();
        conn.Close();
      }
      catch
      {
        throw new ArgumentException("To write the default Tables to the database you need DDL.sql in your path.\n Check if you have it in the right place.");
      }
    }
  }
}