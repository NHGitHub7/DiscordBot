using DiscordBot.Model;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

namespace DiscordBot.Helper
{
  class ConfigurationHelper
  {
    public IConfigurationBuilder Builder()
    {
      try {
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("config.json", optional: false);
        return builder;
      }
      /* 
       * We want to exit the program here.
       * config.json is needed for nearly everything.
       * If it doesn't exist in our path we have to exit.
       */
      catch
      {
        throw new ArgumentException("Error! config.json could not be found in the directory root.\n Check if you have a valid config.json in the same directory as your discord_bot.exe");
      }

    }

    public OAuthorization GetOAuthValue()
    {
      try
      {
        IConfiguration config = Builder().Build();

        var result = config.GetSection("OAuth").Get<OAuthorization>();

        return result;
      }
      catch
      {
        throw new ArgumentException("Error! Check the OAuth section in your config.json.");
      }
    }

    public DB_Access GetDBAccessValues()
    {
      try
      {
        IConfiguration config = Builder().Build();

        var result = config.GetSection("DB").Get<DB_Access>();

        return result;
      }
      catch
      {
        throw new ArgumentException("Error! Check the DB section in your config.json.");
      }
    }

    public Versioning GetVersion()
    {
      try
      {
        IConfiguration config = Builder().Build();

        return config.GetSection("Version").Get<Versioning>();
      }
      catch
      {
        throw new ArgumentException("Error! Check the Version section in your config.json.");
      }
    }

  }
}
