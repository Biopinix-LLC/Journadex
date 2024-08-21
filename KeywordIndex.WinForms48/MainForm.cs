using Journadex.Library;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    public partial class MainForm : Form, IWorkspaceContainer
    {
        private readonly FileMenu _fileMenu;

        public Workspace Workspace { get; } = new Workspace();
        public ProjectForm ProjectForm { get; set; }


        public MainForm()
        {
            InitializeComponent();
            this.IsMdiContainer = true;
            NewProject();
            this.AddWorkspaceSupport(splitContainer1: ProjectForm.journalIndexSplitContainer, splitContainer2: ProjectForm.journalIndexOutlineSplitContainer);
            Directory.CreateDirectory(Constants.AppDataFolder);
            Workspace.BeforeSave += Workspace_BeforeSave;
            Workspace.AfterLoad += Workspace_AfterLoad;
            var files = new IFile[] { ProjectForm.ProjectFile, ProjectForm.IndexesFile, ProjectForm.OutlineFile };
            var commands = new IFileCommands[]
            {
                new FileCommands(NewProject, OpenProject, SaveProject, ProjectForm.PromptForProjectChanges),
                new FileCommands(NewIndexes, OpenIndexes, SaveIndexes, ProjectForm.PromptForIndexChanges),
                new FileCommands(NewOutline, OpenOutline, SaveOutline, ProjectForm.PromptForOutlineChanges)
                };
            _fileMenu = new FileMenu(files, fileToolStripMenuItem, menuStrip, commands, ProjectForm.Close);

        }

        

        
       
       

        private void NewProject()
        {
            if (ProjectForm != null && !ProjectForm.IsDisposed)
            {
                ProjectForm.Disposed += ProjectForm_Disposed;
                ProjectForm?.Close();
                return;
            }
            ProjectForm = new ProjectForm();
            ProjectForm.MdiParent = this;
            ProjectForm.Workspace = Workspace;
            Workspace.ProjectFilePath = ProjectForm.ProjectFile.Path;
            ProjectForm.Show();


        }
        private void SaveIndexes(IFile file)
        {
            ProjectForm.Project.IndexesPath = file.Path;
            ProjectForm.SaveIndexes();
        }

        private void OpenIndexes(IFile file)
        {
            ProjectForm.Project.IndexesPath = file.Path;
            ProjectForm.OpenIndexes();
        }

        private void NewIndexes() => ProjectForm.NewIndexes();
        private void NewOutline() => ProjectForm.NewOutline();

        private void ProjectForm_Disposed(object sender, EventArgs e)
        {
            ProjectForm.Disposed -= ProjectForm_Disposed;
            ProjectForm = null;
            // TODO this isn't always working
            NewProject();
        }

        private void SaveProject(IFile file)
        {
            Workspace.ProjectFilePath = file.Path;
            ProjectForm.SaveProject();
        }

        private void SaveOutline(IFile file)
        {
            ProjectForm.Project.OutlinePath = file.Path;
            ProjectForm.SaveOutline();
        }

        private void OpenProject(IFile file)
        {
            Workspace.ProjectFilePath = file.Path;
            LoadProject();
        }

        private void OpenOutline(IFile file)
        {
            ProjectForm.Project.OutlinePath = file.Path;
            ProjectForm.OpenOutline();
        }




        private void Workspace_AfterLoad(object sender, EventArgs e)
        {
            LoadProject();

        }

        private void LoadProject()
        {
            if (string.IsNullOrEmpty(Workspace.ProjectFilePath)) return;
            ProjectForm.LoadProject();
        }

        private void Workspace_BeforeSave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ProjectForm.JournalText)) return;
            if (string.IsNullOrEmpty(Workspace.ProjectFilePath))
            {
                Workspace.ProjectFilePath = JsonHelpers.GenerateRandomJsonFileName();
            }

            ProjectForm.SaveProject();

        }

        private void PasteClipboardAsJournalToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string clipboard = Clipboard.GetText();
            if (clipboard == null || !clipboard.HasDateLines())
            {
                _ = new MessageDialogBuilder().Message("No valid dates were found in the Clipboard. Please copy text that resembles a journal with entries starting with a date matching the format \"March 1st, 2002\"").Icon(MessageBoxIcon.Warning).Show();
                return;
            }

            if (!string.IsNullOrEmpty(ProjectForm.JournalText))
            {
                var result = new MessageDialogBuilder().Message("This project already has a journal. Would you like to create a new project using the text in the clipboard?")
                    .Buttons(MessageBoxButtons.YesNo).Icon(MessageBoxIcon.Warning).Show();
                if (result == DialogResult.No) return;
                NewProject(); 
                              // TODO error message needs work - splitcontainer for message and details?

            }

            ProjectForm.JournalText = clipboard;
        }



    }
}
