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
      var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("config.json", optional: false);

      return builder;
    }

    public OAuthorization GetOAuthValue()
    {
      IConfiguration config = Builder().Build();

      var result = config.GetSection("OAuth").Get<OAuthorization>();

      return result;
    }

    public DB_Access GetDBAccessValues()
    {
      IConfiguration config = Builder().Build();

      var result = config.GetSection("DB").Get<DB_Access>();

      return result;
    }
    public Versioning GetVersion()
    {
      IConfiguration config = Builder().Build();

      return config.GetSection("Version").Get<Versioning>();
    }

  }
}
