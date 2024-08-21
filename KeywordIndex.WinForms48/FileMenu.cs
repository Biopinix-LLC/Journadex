using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    /// <summary>
    /// ChatGPT: Write a C# class that adds Open, Save and Save As commands to an existing WinForms MenuStrip. It should take an IFile interface as a parameter and show the Open and Save dialogs based on the file type specified by the interface. Action delegates should be used to do the actual Open and Save work.
    /// </summary>
    internal class FileMenu
    {
        private IFile[] _files;       
        private readonly IFileCommands[] _commands;
        private readonly Action _exit;
        private enum FileTypeCommands { New, Open, Save, SaveAs }
        public FileMenu(IFile[] files, ToolStripMenuItem fileMenu, MenuStrip menu, IFileCommands[] commands, Action exit)
        {
            _files = files;
            _commands = commands;
            _exit = exit;
            if (!menu.Items.Contains(fileMenu))
            {
                menu.Items.Add(fileMenu);
            }
            List<ToolStripMenuItem> commandItems = new List<ToolStripMenuItem>();
            foreach (var command in new string[] 
            {
                "New",
                "Open",
                "Save",
                "Save As"
            })
            {
                var commandItem = new ToolStripMenuItem(command);
                commandItems.Add(commandItem);
                List<ToolStripMenuItem> typeItems = new List<ToolStripMenuItem>();
                foreach (var file in _files)
                {
                    var type = new ToolStripMenuItem(file.Type, null, OnTypeClick);
                    typeItems.Add(type);
                }
                commandItem.DropDownItems.AddRange(typeItems.ToArray());                
            }
            
            
            fileMenu.DropDownItems.AddRange(commandItems.ToArray());
            // Add Save As command
            var saveAsItem = new ToolStripMenuItem("Save All", null, SaveAll_Click);
            fileMenu.DropDownItems.Add(saveAsItem);

            // Add Exit command
            var exitItem = new ToolStripMenuItem("Exit", null, Exit_Click);
            fileMenu.DropDownItems.Add(exitItem);
        }

        private void SaveAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _files.Length; i++)
            {
                Save(_files[i], _commands[i]);
            }
        }

        private void OnTypeClick(object sender, EventArgs e)
        {
            var tsmi = sender as ToolStripMenuItem;
            if (!(tsmi.OwnerItem is ToolStripMenuItem parent)) return;
            int typeIndex = parent.DropDownItems.IndexOf(tsmi);
            if (!(parent.OwnerItem is ToolStripMenuItem fileMenu)) return;
            int commandIndex = fileMenu.DropDownItems.IndexOf(parent);
            IFile file = _files[typeIndex];
            IFileCommands command = _commands[typeIndex];
            if (SaveChanges(file, command) == SaveOrCancel.Cancel) return;
            switch ((FileTypeCommands)commandIndex)
            {
                case FileTypeCommands.New:
                    command.New();
                    break;
                case FileTypeCommands.Open:
                    using (var openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = file.Filter;
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            file.Path = openFileDialog.FileName;
                            command.Open(file);
                        }
                    }                    
                    break;
                case FileTypeCommands.Save:
                    if (string.IsNullOrEmpty(file.Path))
                    {
                        SaveAs(file, command);
                    }
                    else
                    {
                        Save(file, command);
                    }
                    break;
                case FileTypeCommands.SaveAs: 
                    SaveAs(file, command);
                    break;
                default:
                    break;
            }



        }

        private void Exit_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _files.Length; i++)
            {
                if (SaveChanges(_files[i], _commands[i]) == SaveOrCancel.Cancel) return;
            }
            _exit();
        }

        private SaveOrCancel SaveChanges(IFile file, IFileCommands commands)
        {
            if (file.HasChanged)
            {
                if (commands.ShouldSaveOrCancel == SaveOrCancel.Save)
                {
                    Save(file, commands);
                    return SaveOrCancel.Save;
                }
                return SaveOrCancel.Cancel;
            }
            return SaveOrCancel.NoChanges;

        }

        private void Save(IFile file, IFileCommands commands) => commands.Save(file);

      
        private void SaveAs(IFile file, IFileCommands commands)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = file.Filter;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    file.Path = saveFileDialog.FileName;
                    Save(file, commands);
                }
            }
        }
    }
}
