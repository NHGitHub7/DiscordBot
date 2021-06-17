using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.DB;
using DSharpPlus.Entities;

namespace DiscordBot.Swearwords
{
  class Blacklist
  {
    static List<String> swearwords_cache;
    static Int16 counter;

    public static void init()
    {
      swearwords_cache = Database.get_swearwords();
      counter = 0;
    }
    public static bool is_swearword(string word)
    {
      List<string> swearwords;
      if (counter >= 1000)
      {
        swearwords = Database.get_swearwords();
        swearwords_cache = swearwords;
        counter = 0;
      }
      else
      {
        swearwords = swearwords_cache;
        counter++;
      }

      foreach (var i in swearwords)
      {
        if (word.Contains(i))
        {
          return true;
        }
      }
      return false;
    }
    public static string get_strike_msg(string mention, int strikes)
    {
      if (strikes == 2)
      {
        return $"@everyone, {mention} cussed too much and is now banned. \n Let this be a warning to you!";
      }
      else if (strikes > 2)
      {
        return $"{mention} can't stop cussing, can he ? {++strikes}/3 ";
      }
      else
      {
        return $"{mention} You now have {++strikes}/3 strikes! If you use such cusses again you will be banned!";
      }
    }

    public static int get_strikes(UInt64 user_id)
    {
      object strikes = Database.get_strikes_from_user(user_id);
      if (strikes == null)
      {
        return 0;
      }
      else
      {
        return (int)strikes;
      }
    }

    public static void update_strikes(int strikes, UInt64 user_id)
    {
      if (strikes == 0)
      {
        string query = "INSERT INTO swearword_strikes(user_id, strikes) " +
          $"VALUES ({user_id}, 1)";
        Database.runScalar(query);
      }
      else
      {
        string query = "UPDATE swearword_strikes " +
          $"SET strikes = {++strikes} " +
          $"WHERE user_id = {user_id}";
        Database.runScalar(query);
      }
    }

    public static async Task ban_member(UInt64 user_id, DiscordGuild guild)
    {
      await guild.BanMemberAsync(user_id, 0, "You cussed too much");
    }

    public static async Task strike_user(DiscordMessage message, UInt64 user_id, DiscordGuild guild, DiscordChannel channel, string mention)
    {
      await message.DeleteAsync();
      int strikes = Blacklist.get_strikes(user_id);
      Blacklist.update_strikes(strikes, user_id);
      // 1,2,banned
      if (strikes >= 2)
      {
        await Blacklist.ban_member(user_id, guild);
      }
      await channel.SendMessageAsync(Blacklist.get_strike_msg(mention, strikes));
    }
  }
}
