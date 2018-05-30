// ---------------------------------------------------------
// Copyrights (c) 2014-2018 LeafyDev 🍂 All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Threading;

namespace RequestBotThing
{
    internal static class Logger
    {
        private static readonly ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

        internal static void WriteToFileThreadSafe(string text)
        {
            // Set Status to Locked
            _readWriteLock.EnterWriteLock();
            try
            {
                // Append text to the file
                using (var sw = File.AppendText("latest.log"))
                {
                    sw.WriteLine(text);
                    sw.Close();
                }
            }
            finally
            {
                // Release lock
                _readWriteLock.ExitWriteLock();
            }
        }
    }
}