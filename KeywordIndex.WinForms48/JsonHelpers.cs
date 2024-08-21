using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace KeywordIndex.WinForms48
{
    internal static class JsonHelpers
    {
        public static string GenerateRandomJsonFileName()
        {
           
            // Generate a random file name
            string fileName = Path.GetRandomFileName();
            fileName = Path.ChangeExtension(fileName, "json");

            // Create the full file path
            string filePath = Path.Combine(Constants.AppDataFolder, fileName);

            return filePath;
        }

        public static T LoadJson<T>(string filePath) =>
            JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath, Encoding.UTF8));

        public static void SaveJson<T>(T obj, string filePath) =>
            File.WriteAllText(filePath, JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), Encoding.UTF8);
    }
}