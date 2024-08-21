using System.Windows.Forms;
using System;

namespace KeywordIndex.WinForms48
{
    public interface IFileContainer<T> where T : IFile
    {
        IFile AsFile();
    }

    public static class IFileContainerExensions
    {
        internal static SaveOrCancel PromptForChanges<T>(this IFileContainer<T> fileContainer) where T : IFile  
        {
            if (fileContainer.AsFile().HasChanged)
            {
                var result = MessageBox.Show("Save Changes?", "Save", buttons: MessageBoxButtons.YesNoCancel, icon: MessageBoxIcon.Warning);
                switch (result)
                {

                    case DialogResult.Yes:
                        return SaveOrCancel.Save;
                    case DialogResult.No:
                        return SaveOrCancel.NoChanges;
                    case DialogResult.Cancel:
                        return SaveOrCancel.Cancel;
                    default:
                        throw new NotSupportedException();
                }
            }
            return SaveOrCancel.NoChanges;
        }
    }
}