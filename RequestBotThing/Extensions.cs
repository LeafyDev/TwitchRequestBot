// ---------------------------------------------------------
// Copyrights (c) 2014-2018 LeafyDev 🍂 All rights reserved.
// ---------------------------------------------------------

using TwitchLib.Client.Models;

namespace RequestBotThing
{
    internal static class Extensions
    {
        public static string Parse(this string input, string user = "", string channel = "", string requestNumber = "",
            string amount = "", string tier = "")
        {
            var result = input;

            if (!string.IsNullOrEmpty(user))
                result = result.Replace("[user]", user);

            if (!string.IsNullOrEmpty(channel))
                result = result.Replace("[channel]", channel);

            if (!string.IsNullOrEmpty(requestNumber))
                result = result.Replace("[requestNumber]", requestNumber);

            if (!string.IsNullOrEmpty(amount))
                result = result.Replace("[amount]", amount);

            if (!string.IsNullOrEmpty(tier))
                result = result.Replace("[tier]", tier);

            return result;
        }

        public static bool IsMod(this ChatMessage msg) => msg.IsModerator || msg.Username.ToLower() == "leafydev";
    }
}