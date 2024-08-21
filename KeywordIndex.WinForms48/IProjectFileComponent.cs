namespace KeywordIndex.WinForms48
{
    internal interface IProjectFileComponent
    {
        void LoadFromProject(Project project);
        void SaveToProject(Project project);
    }
}