using DiscordBot.Model;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DiscordBot.Helper
{
    class ConfigurationHelper
    {
        public IConfigurationBuilder Builder()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("configuration.json", optional: false);

            return builder;
        }

        public OAuthorization GetOAuthValue()
        {
            IConfiguration config = Builder().Build();

            var result = config.GetSection("OAuth").Get<OAuthorization>();

            return result;
        }
    }
}
