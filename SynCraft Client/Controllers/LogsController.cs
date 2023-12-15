using SynCraftClient.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SynCraftClient.Controllers
{
    internal class LogsController
    {
        private static readonly string FilePath = Directory.GetCurrentDirectory() + "\\SynCraftClient.log";
        private static object Locker = new();

        public static void Init()
        {
            Logs.All = "";
            Logs.Queue = [];
        }

        private static void WriteToFile(string log)
        { 
            lock (Locker)
            {
                File.AppendAllText(FilePath, log);
            }
        }

        public static void Info(string message)
        {
            string finalLog = "[" + DateTime.Now.ToString() + "]" +
                "[INFO]: " + message;

            Logs.All += finalLog + "\n";
            //Logs.Queue?.Add(finalLog);

            if (Settings.LogToFile)
                Task.Run(() => WriteToFile(finalLog));
        }

        public static void Warn(string message)
        {
            string finalLog = "[" + DateTime.Now.ToString() + "]" +
                "[WARN]: " + message;

            Logs.All += finalLog + "\n";

            if (Settings.LogToFile)
                Task.Run(() => WriteToFile(finalLog));
        }

        public static void Error(string message)
        {
            string finalLog = "[" + DateTime.Now.ToString() + "]" +
                "[ERROR]: " + message;

            Logs.All += finalLog + "\n";

            if (Settings.LogToFile)
                Task.Run(() => WriteToFile(finalLog));
        }
    }
}
