using Journadex.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    public partial class ProjectForm : Form
    {
        private const string SavingCaption = "Saving...";
        private readonly KeywordIndexer _keywordIndexer;
        private readonly DateIndexer _dateIndexer;
        private readonly Outline _outline;
        private readonly Navigator _navigator;
        private readonly PageNavigator _pageNavigator;
        private FindPanel _findPanel;

        internal Project Project { get; private set; } = new Project();
        internal Indexes Indexes { get; private set; } = new Indexes();
        internal OutlineData Outline { get; private set; } = new OutlineData();
        internal Workspace Workspace { get; set; }
        internal IFile ProjectFile => Project.AsFile();
        internal IFile IndexesFile => Indexes.AsFile();
        internal IFile OutlineFile => Outline.AsFile();

        public string JournalText
        {
            get => journalRtf.Text;
            internal set
            {
                //journalRtf.Text = _pageNavigator.Build(value);
                journalRtf.Text = value;
            }
        }

        internal CursorCoordinator CursorCoordinator { get; }

        public ProjectForm()
        {
            InitializeComponent();
            _keywordIndexer = new KeywordIndexer(journalRtf, keywordIndexBrowser);
            _keywordIndexer.ShowCheckboxes = true;
            _keywordIndexer.CheckBoxSelectionChanged += CheckBoxSelection_SelectionChanged;
            _keywordIndexer.IndexChanged += Indexer_IndexChanged;
            _dateIndexer = new DateIndexer(journalRtf, monthCalendar, dateIndex);            
            _dateIndexer.ShowCheckboxes = true; 
            _dateIndexer.CheckBoxSelectionChanged += CheckBoxSelection_SelectionChanged;
            _keywordIndexer.GetIndexRangeDescription = _dateIndexer.RangeDescriptors.DateWithSnippet;
            _outline = new Outline(journalRtf, outlineBrowser);
            _keywordIndexer.RangeSelectionDestinationProvider = _outline;
            _dateIndexer.RangeSelectionDestinationProvider = _outline;
            _navigator = new Navigator(toolStrip, journalRtf);
            _pageNavigator = new PageNavigator(toolStrip, journalRtf);
            journalRtf.TextChanged += JournalRtf_TextChanged;
            CursorCoordinator = new CursorCoordinator(_keywordIndexer, _dateIndexer, _pageNavigator, _navigator, journalRtf, _outline);            
            indexTabs.SelectedTab = dateTab; //  TODO not sure why but moving the date tab to be first in the designer broke things, even though it was the only change (events not affected)
            CursorCoordinator.History.IndexChanged += Indexer_IndexChanged;

        }       


        private void JournalRtf_TextChanged(object sender, EventArgs e)
        {
            journalRtf.TextChanged -= JournalRtf_TextChanged;
            _dateIndexer.PopulateCalendar();
        }

        private void Indexer_IndexChanged(object sender, RangeListChangedEventArgs e)
        {
            if (e.ItemAdded == ListChangedType.ItemAdded)
            {
                _navigator.AddKeyword(e.IndexKey, e.Range);
            }
            else if (e.ItemAdded == ListChangedType.ItemDeleted)
            {
                _navigator.RemoveKeyword(e.IndexKey, e.Range);
            }
            else throw new NotSupportedException();
        }

        internal SaveOrCancel PromptForProjectChanges() => Project.PromptForChanges();


        internal void LoadProject()
        {

            ProgressManager.Show();
            CursorCoordinator.On = false;
            try
            {
                Project = JsonHelpers.LoadJson<Project>(Workspace.ProjectFilePath);
                OpenIndexes(showProgress: false);
                OpenOutline(showProgress: false);
                if (Project.CursorPosition > 0)
                {
                    journalRtf.Select(Project.CursorPosition, Project.SelectionLength);
                    CursorCoordinator.History.Clear();
                    CursorCoordinator.History.AddSelectedRange();
                    CursorCoordinator.SyncComponents();
                }

            }
            catch (FileNotFoundException)
            {
                Workspace.ProjectFilePath = null;
            }
            finally
            {
                CursorCoordinator.On = true;
                ProgressManager.Hide();
            }

        }

        internal void OpenIndexes(bool showProgress = true)
        {
            using (new DisposableProgress(showProgress: showProgress))
            {
                if (Indexes.Journal == null && !string.IsNullOrEmpty(Project.IndexesPath))
                {
                    Indexes = JsonHelpers.LoadJson<Indexes>(Project.IndexesPath);
                    _pageNavigator.LoadFromIndexes(Indexes);
                    _dateIndexer.LoadFromIndexes(Indexes);
                    _keywordIndexer.LoadFromIndexes(Indexes);
                }
                                                         
                _navigator.AddFromRangeList(_keywordIndexer);
                _navigator.AddFromRangeList(_dateIndexer);
                _navigator.AddFromRangeList(CursorCoordinator.History);
                _navigator.Select(DateIndexer.EntriesKey);
            }
        }

        internal void OpenOutline(bool showProgress = true)
        {
            using (new DisposableProgress(showProgress: showProgress))
            {
                if (Outline.Outline == null && !string.IsNullOrEmpty(Project.OutlinePath))
                {
                    Outline = JsonHelpers.LoadJson<OutlineData>(Project.OutlinePath);
                    _outline.LoadFromOutline(Outline);
                    _keywordIndexer.UpdateIndex();
                    _dateIndexer.UpdateIndex();
                }
               
            }
        }

        internal void SaveProject()
        {
            if (string.IsNullOrEmpty(Workspace.ProjectFilePath)) return;            
            using (new DisposableProgress(SavingCaption))
            {
                SaveIndexes(showProgress: false);
                SaveOutline(showProgress: false);
                Project.CursorPosition = journalRtf.SelectionStart;
                Project.SelectionLength = journalRtf.SelectionLength;
                JsonHelpers.SaveJson(Project, Workspace.ProjectFilePath);
            }
            
        }

        internal void SaveOutline(bool showProgress = true)
        {
            if (string.IsNullOrEmpty(Project.OutlinePath)) return;
            using (new DisposableProgress(SavingCaption, showProgress))
            {
                _outline.SaveToOutline(Outline);
                JsonHelpers.SaveJson(Outline, Project.OutlinePath);
            }
        }

        internal void NewOutline()
        {
            throw new NotImplementedException();
        }

        internal void SaveIndexes(bool showProgress = true)
        {
            if (string.IsNullOrEmpty(Project.IndexesPath)) return;
            using (new DisposableProgress(SavingCaption, showProgress))
            {
                _pageNavigator.SaveToIndexes(Indexes);
                _keywordIndexer.SaveToIndexes(Indexes);
                _dateIndexer.SaveToIndexes(Indexes);
                JsonHelpers.SaveJson(Indexes, Project.IndexesPath);
            }
        }

      
        internal void NewIndexes()
        {
            throw new NotImplementedException();
        }

        private void CheckBoxSelection_SelectionChanged(object sender, EventArgs e)
        {
            if (!(sender is Indexer indexer)) return;
            //RefreshSelectedRanges(indexer);

        }

        //private void RefreshSelectedRanges(Indexer indexer)
        //{
        //    Range[] keepSelectedRanges = _outline.UpdateFromIndex(indexer, indexer == _dateIndexer);
        //    if (keepSelectedRanges.Length > 0)
        //    {
        //        indexer.AddSelectedRanges(keepSelectedRanges);
        //    }
        //}

        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {

            if (string.IsNullOrEmpty(journalRtf.SelectedText) || !journalRtf.IsCursorInsideSelection())
            {
                int index = journalRtf.GetCharIndexFromPosition(journalRtf.PointToClient(Cursor.Position));
                journalRtf.SelectCurrentWord(index);
            }
            if (journalRtf.SelectedText.Length <= 1)
            {
                e.Cancel = true;
                return;
            }

            for (int i = tagsMenu.DropDownItems.Count - 1; i >= 2; i--)
            {
                tagsMenu.DropDownItems.RemoveAt(i);
            }
            var exists = _keywordIndexer.Contains(journalRtf.SelectedText);
            addToIndex.Visible = !exists;
            removeFromIndex.Visible = exists;
            List<ToolStripMenuItem> tagItems = (from kvp in _keywordIndexer.GetTagsForSelectedText()
                                                select new ToolStripMenuItem
                                                {
                                                    Text = kvp.Key,
                                                    Tag = kvp.Value,
                                                    Checked = true,
                                                    CheckOnClick = true
                                                }).ToList();

            if (tagItems.Count > 0)
            {
                tagItems.ForEach(tagItem => tagItem.CheckedChanged += TagItem_CheckedChanged);
                tagsSeparator.Visible = true;
                tagsMenu.DropDownItems.AddRange(tagItems.ToArray());

            }
            else
            {
                tagsSeparator.Visible = false;

            }
            //addExistingTag.Items.Clear();
            //addExistingTag.Items.AddRange(_indexer.GetKeys());
            if (_keywordIndexer.ItemCount == 0)
            {
                aliasOf.Visible = aliasLabel.Visible = aliasSeparator.Visible = false;
                return;
            }
            aliasOf.Visible = aliasLabel.Visible = aliasSeparator.Visible = true;
            aliasOf.ComboBox.Text = string.Empty;
            bool existsAsAlias = _keywordIndexer.TryGetSelectedTextAsAlias(out string indexItem);
            aliasOf.Items.Clear();
            if (existsAsAlias)
            {
                aliasOf.ComboBox.Text = indexItem;
                aliasOf.Enabled = false;
            }
            else
            {
                aliasOf.Enabled = true;
                aliasOf.Items.AddRange(_keywordIndexer.GetKeys());
            }
        }

        private void TagItem_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem item) || !(item.Tag is Range range)) return;
            if (MessageBox.Show($"Remove the {item.Text} tag from this selection?", "Remove Tag?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            _keywordIndexer.RemoveTagRange(item.Text, range, out int rangesCount);
            if (rangesCount == 0 && MessageBox.Show("There are no more selections referencing this tag. Do you want to remove it from the index?", "Remove from Index?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            _keywordIndexer.Remove(item.Text);
            item.CheckedChanged -= TagItem_CheckedChanged;

        }

        private void AddToIndex_Click(object sender, EventArgs e)
        {
            CursorCoordinator.On = false;
            try
            {
                _keywordIndexer.AddSelectedToIndex();

            }
            finally
            {
                CursorCoordinator.On = true;

            }
        }

        private void RemoveFromIndex_Click(object sender, EventArgs e)
        {
            CursorCoordinator.On = false;
            try
            {
                _keywordIndexer.RemoveSelectedFromIndex();

            }
            finally
            {
                CursorCoordinator.On = true;

            }
        }

        private void AliasOf_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (aliasOf.SelectedItem is string indexItem && !string.IsNullOrEmpty(indexItem))
            {
                _keywordIndexer.AddSelectedAsAliasOf(indexItem);

            }
        }

        private void AddTag_Click(object sender, EventArgs e)
        {
            string defaultResult = journalRtf.SelectedText.Trim();
            string[] options = new string[] { $"Add tag to all \"{defaultResult}\" in document" };
            _keywordIndexer.ShowAddTagDialog(out ComboBox parent, out Tuple<string, bool[]> result, options, defaultResult);
            if (result?.Item1 is string tagName && !string.IsNullOrEmpty(tagName))
            {
                bool toAllMatchingSelected = result.Item2[0];
                _keywordIndexer.AddSelectedAsTagToIndex(tagName, toAllMatchingSelected, isTag: true, parent.Text);
            }
        }

        private void CleanHighlights_Click(object sender, EventArgs e) => _keywordIndexer.CleanupHighlights();

        private void Copy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(journalRtf.SelectedText))
            {
                ClipboardWithMessage.SetText(journalRtf.SelectedText, journalRtf);
            }
        }

        private void CopyWithDateReferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(journalRtf.SelectedText))
            {
                var date = _dateIndexer.GetDateForRange(journalRtf.GetSelectedRange());
                StringBuilder sb = new StringBuilder();
                if (date != null)
                {
                    sb.Append(date.Value.ToString("d MMM yyyy")).Append(" - ");

                }
                sb.Append(journalRtf.SelectedText);
                ClipboardWithMessage.SetText(sb.ToString(), journalRtf);
            }
        }

       
        internal SaveOrCancel PromptForIndexChanges() => Indexes.PromptForChanges();

        internal SaveOrCancel PromptForOutlineChanges() => Outline.PromptForChanges();

        private void FindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_findPanel == null)
            {
                _findPanel = FindPanel.Show(journalRtf);
            }
            else
            {
                _findPanel.ShowPanel();
            }

        }
    }
}
