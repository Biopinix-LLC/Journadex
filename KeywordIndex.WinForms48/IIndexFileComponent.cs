namespace KeywordIndex.WinForms48
{
    internal interface IIndexFileComponent
    {
        void LoadFromIndexes(IIndexData indexData);
        void SaveToIndexes(IIndexData indexData);
    }
}