// ---------------------------------------------------------
// Copyrights (c) 2014-2018 LeafyDev 🍂 All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Net;

namespace RequestBotThing
{
    internal static class Updates
    {
        public static bool Error;
        public static string webVersion;

        public static bool Check(string version)
        {
            Error = false;

            var webClient = new WebClient();
            var stream = webClient.OpenRead("http://g.whaskell.pw/requestbot/latest.txt");
            if (stream != null)
            {
                var reader = new StreamReader(stream);
                webVersion = reader.ReadToEnd().Replace("\n", "");

                return webVersion == version;
            }

            Error = true;

            return false;
        }
    }
}