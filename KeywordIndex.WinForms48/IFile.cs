namespace KeywordIndex.WinForms48
{
    public interface IFile
    {
        string Filter { get; }
        string Path { get; set; }
        bool HasChanged { get; }
        string Type { get; }
    }
}