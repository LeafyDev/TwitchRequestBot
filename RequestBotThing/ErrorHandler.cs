// ---------------------------------------------------------
// Copyrights (c) 2014-2018 LeafyDev 🍂 All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Windows.Forms;

namespace RequestBotThing
{
    internal static class ErrorHandler
    {
        public static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (e.Exception is Exception ex)
                MessageBox.Show($@"{ex.GetType()}{Environment.NewLine}{Environment.NewLine}{ex}");
        }

        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                MessageBox.Show($@"{ex.GetType()}{Environment.NewLine}{Environment.NewLine}{ex}");
        }
    }
}