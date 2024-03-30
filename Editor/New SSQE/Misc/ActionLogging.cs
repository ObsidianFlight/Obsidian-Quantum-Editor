using System;
using System.Collections.Generic;
using System.IO;

namespace New_SSQE
{
    // for debugging purposes
    internal class ActionLogging
    {
        public static List<string> Logs = new();

        public static void Register(string log, string tag = "INFO", Exception? ex = null)
        {
            if (ex != null)
                log += $"\n\n{ExtractExceptionInfo(ex)}";

            var timestamp = DateTime.Now;
            var logF = $"[{timestamp} - {tag.ToUpper()}] {log}";

            Logs.Add(logF);

            if (Settings.settings["debugMode"])
            {
                var logs = string.Join('\n', Logs);
                File.WriteAllText("logs-debug.txt", logs);
            }
        }

        public static string ExtractExceptionInfo(Exception e)
        {
            Exception? ex = e;

            List<string> msg = new();

            while (ex != null)
            {
                msg.Add($"{e.Message}\n\n{e.StackTrace ?? "[StackTrace was null]"}");
                ex = ex.InnerException;
            }

            return string.Join("\n\n", msg);
        }
    }
}
