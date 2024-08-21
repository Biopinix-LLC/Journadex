using Journadex.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    public class Workspace
    {
        public string ProjectFilePath { get; set; }
        public Rectangle FormBounds { get; set; }
        public FormWindowState WindowState { get; set; }
        public int SplitContainer1Distance { get; set; }
        public int SplitContainer2Distance { get; set; }
        internal event EventHandler BeforeSave;
        internal event EventHandler AfterLoad;

        internal void OnBeforeSave() => BeforeSave?.Invoke(this, EventArgs.Empty);

        internal void OnAfterLoad() => AfterLoad?.Invoke(this, EventArgs.Empty);
    }

    public static class WorkspaceForm
    {
        public static void AddWorkspaceSupport<T>(this T form, SplitContainer splitContainer1, SplitContainer splitContainer2) where T : Form, IWorkspaceContainer
        {
            form.Load += Form_Load;
            form.FormClosing += Form_FormClosing;

            void Form_Load(object sender, EventArgs e)
            {
                if (File.Exists(Constants.FilePath))
                {
                    Workspace loadedWorkspace = JsonHelpers.LoadJson<Workspace>(Constants.FilePath);
                    Form child = GetChild(form, sender);
                    child.Bounds = loadedWorkspace.FormBounds;
                    child.WindowState = loadedWorkspace.WindowState;
                    splitContainer2.SplitterDistance = loadedWorkspace.SplitContainer2Distance;
                    splitContainer1.SplitterDistance = loadedWorkspace.SplitContainer1Distance;

                    // Update the workspace object with the loaded values
                    form.Workspace.ProjectFilePath = loadedWorkspace.ProjectFilePath;
                    form.Workspace.FormBounds = loadedWorkspace.FormBounds;
                    form.Workspace.WindowState = loadedWorkspace.WindowState;
                    form.Workspace.SplitContainer2Distance = loadedWorkspace.SplitContainer2Distance;
                    form.Workspace.SplitContainer1Distance = loadedWorkspace.SplitContainer1Distance;
                    form.Workspace.OnAfterLoad();
                }
            }

            void Form_FormClosing(object sender, FormClosingEventArgs e)
            {
                Form child = GetChild(form, sender);
                form.Workspace.FormBounds = child.Bounds;
                form.Workspace.WindowState = child.WindowState;
                form.Workspace.SplitContainer1Distance = splitContainer1.SplitterDistance;
                form.Workspace.SplitContainer2Distance = splitContainer2.SplitterDistance;
                form.Workspace.OnBeforeSave();
                JsonHelpers.SaveJson(form.Workspace, Constants.FilePath);
            }
        }

        private static Form GetChild<T>(T form, object sender) where T : Form, IWorkspaceContainer
        {
            return sender is Form parent && parent.IsMdiContainer ? parent.MdiChildren[0] : form;
        }
    }
}
