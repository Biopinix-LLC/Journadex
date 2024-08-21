namespace KeywordIndex.WinForms48
{
    public enum SaveOrCancel {
        NoChanges, Save, Cancel,
        
    }
    internal interface IFileCommands
    {
        SaveOrCancel ShouldSaveOrCancel { get; }

        void New();
        void Open(IFile file);
        void Save(IFile file);
    }
}