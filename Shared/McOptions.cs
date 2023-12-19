using System.Collections.Generic;
using System.IO;

namespace Shared
{
    public class McOptions
    {
        public static Dictionary<string, string> Get(string filePath)
        {
            Dictionary<string, string> options = [];
            string fileContent = File.ReadAllText(filePath);

            foreach (string line in fileContent.Split("\n"))
            {
                if (line.IndexOf(":") != -1) // IndexOf() is 5x faster than Contains()
                    options.Add(line.Split(":")[0], line.Split(":")[1]);
            }

            return options;
        }

        public static void MergeAndWrite(string filePath, Dictionary<string, string> optionsToMerge)
        {
            Dictionary<string, string> currentOptions = Get(filePath);
            string fileContent = "";

            foreach (KeyValuePair<string, string> opt in optionsToMerge)
            {
                if (currentOptions.ContainsKey(opt.Key))
                    currentOptions[opt.Key] = opt.Value;
                else
                    currentOptions.Add(opt.Key, opt.Value);
            }

            foreach (KeyValuePair<string, string> opt in currentOptions)
                fileContent += opt.Key + ":" + opt.Value + "\n";

            File.WriteAllText(filePath, fileContent);
        }
    }
}
