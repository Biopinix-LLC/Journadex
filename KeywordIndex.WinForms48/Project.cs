using Journadex.Library;
using System;
using System.Collections.Generic;

namespace KeywordIndex.WinForms48
{
    public class Project : IFileContainer<Project.ProjectFile>
    {
        public const string ProjectFileExtension = "json";

        public int CursorPosition { get; set; }
        public string OutlinePath { get; set; }
        public string IndexesPath { get; set; }
        public int SelectionLength { get; set; }

        private ProjectFile _file = new ProjectFile();
        internal class ProjectFile : IFile
        {

            public string Filter => $"Project File (*.{ProjectFileExtension})|*.{ProjectFileExtension}";

            public string Path { get; set; }

            public bool HasChanged { get; private set; }

            public string Type => nameof(Project);
        }

        public IFile AsFile() => _file;

        
    }
}