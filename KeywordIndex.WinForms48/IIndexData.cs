namespace KeywordIndex.WinForms48
{
    public interface IIndexData
    {
        string Text { get; set; }
        string Rtf { get; set; }
        DateIndexer.JournalInfo Journal { get; set; }
        KeywordIndexer.KeywordInfo Keywords { get; set; }
    }
}