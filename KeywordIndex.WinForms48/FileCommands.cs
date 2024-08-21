using System;

namespace KeywordIndex.WinForms48
{
    internal class FileCommands : IFileCommands
    {
        private readonly Action newFile;
        private Action<IFile> openFile;
        private Action<IFile> saveFile;
        private readonly Func<SaveOrCancel> saveOrCancel;

        public FileCommands(Action newFile, Action<IFile> openFile, Action<IFile> saveFile, Func<SaveOrCancel> saveOrCancel)
        {
            this.newFile = newFile;
            this.openFile = openFile;
            this.saveFile = saveFile;
            this.saveOrCancel = saveOrCancel;
        }

        public SaveOrCancel ShouldSaveOrCancel => saveOrCancel();

        public void New() => newFile();


        public void Open(IFile file) => openFile(file);

        public void Save(IFile file) => saveFile(file);
    }
}