namespace KeywordIndex.WinForms48
{

    internal class Indexes : IFileContainer<Indexes.IndexesFile>, IIndexData
    {
        public const string IndexesFileExtension = "json";

        public string Text { get; set; }
        public string Rtf { get; set; }
        public DateIndexer.JournalInfo Journal { get; set; }
        public KeywordIndexer.KeywordInfo Keywords { get; set; }
        private IndexesFile _indexesFile = new IndexesFile();
        public IFile AsFile()
        {
            return _indexesFile;
        }

        internal class IndexesFile : IFile
        {

            public string Filter => $"Indexes File (*.{IndexesFileExtension})|*.{IndexesFileExtension}";

            public string Path { get; set; }

            public bool HasChanged { get; private set; }

            public string Type => nameof(Indexes);
        }
    }
}
