using System;
using System.IO;
using System.Threading.Tasks;

namespace Shared
{
    public static class Logger
    {
        public static string AllLogs = "";
        public static bool LogToFile;

        private static readonly string FilePath = Directory.GetCurrentDirectory() + "\\SynCraftClient.log";
        private static object Locker = new();

        private static void WriteToFile(string log)
        {
            lock (Locker)
            {
                File.AppendAllText(FilePath, log);
            }
        }

        private static void ProcessLog(string finalLog)
        {
            AllLogs += finalLog + "\n";

            if (LogToFile)
                Task.Run(() => WriteToFile(finalLog));
        }

        public static void Info(string message)
        {
            ProcessLog("[" + DateTime.Now.ToString() + "]" +
                "[INFO]: " + message);
        }

        public static void Warn(string message)
        {
            ProcessLog("[" + DateTime.Now.ToString() + "]" +
                "[WARN]: " + message);
        }

        public static void Error(string message)
        {
            ProcessLog("[" + DateTime.Now.ToString() + "]" +
                "[ERROR]: " + message);
        }
    }
}
