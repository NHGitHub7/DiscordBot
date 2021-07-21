using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.DB;
using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.EventArgs;

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

    /**
     * check if the currently written message contains a swearword.
     *
     * exclude check for strings that include the commands for adding & removing cusses.
     * But only if the User has the Permission to use the commands.
     */
    public static bool is_swearword(string word, MessageCreateEventArgs e)
    {
      List<string> swearwords;

      // who knows how good C# is at keeping count. Refresh is sanity.
      if (counter >= 50)
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
          if (word.Contains("!cussadd") || word.Contains("!cussrm"))
          {
            /**
             * Checks if the User has the Administrator Permission
             */
            Permissions needed_perm = Permissions.Administrator;
            DiscordMember user = e.Guild.Members[e.Author.Id];
            Permissions user_perm = user.PermissionsIn(e.Channel);
            bool perm = user_perm.HasPermission(needed_perm);
            if (perm)
            {
              return false;
            }
            else
            {
              return true;
            }
          }
          return true;
        }
      }
      return false;
    }

    /**
     * Generate Messages for striking a User.
     */
    static string get_strike_msg(string mention, int strikes)
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

    /**
     * Get the Strikes of a User
     */
    static int get_strikes(UInt64 user_id)
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

    /**
     * Update strikes of a user (Set to 1 or increment)
     */
    static void update_strikes(int strikes, UInt64 user_id)
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

    /**
     * Users are banned if they swear more than 3 times.
     * Banning happens here.
     */
    static async Task ban_member(UInt64 user_id, DiscordGuild guild)
    {
      await guild.BanMemberAsync(user_id, 0, "You cussed too much");
    }

    /**
     * Delete the message with the cuss and also give the user a strike.
     * Ban the User if he cussed 3 times.
     */
    public static async Task strike_user(MessageCreateEventArgs e)
    {
      try
      {
        await e.Message.DeleteAsync();
      }
      catch (Exception exc)
      {
        Console.WriteLine(exc);
      }

      int strikes = Blacklist.get_strikes(e.Author.Id);
      Blacklist.update_strikes(strikes, e.Author.Id);
      // 1,2,banned
      if (strikes >= 2)
      {
        await Blacklist.ban_member(e.Author.Id, e.Guild);
      }
      await e.Channel.SendMessageAsync(Blacklist.get_strike_msg(e.Author.Mention, strikes));
    }
  }
}
