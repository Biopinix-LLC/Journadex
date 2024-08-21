using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Journadex.Library
{
    
    /// <summary>
    /// ChatGPT: Create a C# class that saves a Workspace object tp a JSON file in the standard application data folder and loads the object if it exists.
    /// </summary>
    public static class WorkspaceManager<T> where T : class
    {
        private static readonly string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string FilePath = Path.Combine(AppDataFolder, "workspace.json");

        public static void Save(T workspace)
        {
            string json = JsonConvert.SerializeObject(workspace, Formatting.Indented);
            File.WriteAllText(FilePath, json, Encoding.UTF8);
        }

        public static T Load()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath, Encoding.UTF8);
                return JsonConvert.DeserializeObject<T>(json);
            }

            return null;
        }
    }
}
