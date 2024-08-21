namespace KeywordIndex.WinForms48
{
    public class OutlineData : IFileContainer<OutlineData.OutlineFile>, IOutlineData
    {
        public const string FileExtension = "json";
        public Outline.OutlineInfo Outline { get; set; }
        private OutlineFile _file = new OutlineFile();
        public IFile AsFile() => _file;

        internal class OutlineFile : IFile
        {

            public string Filter => $"Outline File (*.{FileExtension})|*.{FileExtension}";

            public string Path { get; set; }

            public bool HasChanged { get; private set; }

            public string Type => nameof(Outline);

        }
    }
}