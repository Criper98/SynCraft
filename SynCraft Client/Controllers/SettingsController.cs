using Newtonsoft.Json.Linq;
using SynCraftClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SynCraftClient.Controllers
{
    internal class SettingsController
    {
        private static readonly string FilePath = Directory.GetCurrentDirectory() + "\\Settings.json";

        public static void Save()
        {
            JObject json = [];

            FieldInfo[] fields = typeof(Settings).GetFields();

            foreach (FieldInfo fi in fields)
                json[fi.Name] = JToken.FromObject(fi.GetValue(null));

            File.WriteAllText(FilePath, json.ToString());
        }

        public static void Read()
        {
            if (!File.Exists(FilePath))
            {
                Init();
                return;
            }

            string fileContent = File.ReadAllText(FilePath);

            JObject json = JObject.Parse(fileContent);

            FieldInfo[] fields = typeof(Settings).GetFields();

            foreach (FieldInfo fi in fields)
            {
                if (json.TryGetValue(fi.Name, out JToken? jk))
                    fi.SetValue(null, jk.ToObject(fi.FieldType));
            }
        }

        private static void Init()
        {
            Settings.ServerPort = 7575;
            Settings.ServerAddress = "127.0.0.1";
            Settings.SyncMods = true;
            Settings.SyncConfig = true;
            Settings.MinecraftPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\.minecraft";
            Settings.LogToFile = true;

            Save();
        }
    }
}
