using System;
using System.IO;

namespace KeywordIndex.WinForms48
{
    internal static class Constants
    {
        private const string AppName = "Journadex";
        internal static readonly string AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
        internal static readonly string FilePath = Path.Combine(AppDataFolder, "workspace.json");
    }
}