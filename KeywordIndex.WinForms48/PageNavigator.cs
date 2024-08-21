using Journadex.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    /// <summary>
    /// ChatGPT: Create a C# class named PageNavigator takes an existing WinForms RichTextBox and ToolStrip and uses an instance of the Journal class to navigate to different pages within the text via textboxes on the toolstrip that display the current volume and page number. The class should also add the necessary tool strip items. I would also like the volume and page textboxes to automatically update when the richtextbox cursor changes position.
    /// </summary>
    internal class PageNavigator : IIndexFileComponent, IProjectFileComponent
    {
        private JournalText _journalText;
        private RichTextBox _richTextBox;
        private ToolStrip _toolStrip;
        private ToolStripTextBox _volumeTextBox;
        private ToolStripTextBox _pageTextBox;
        private CursorCoordinator _cursorCoordinator;

        internal CursorCoordinator CursorCoordinator 
        { 
            set
            {
                _cursorCoordinator = value;
            }
        }

        public PageNavigator(ToolStrip toolStrip, RichTextBox richTextBox)
        {
            _richTextBox = richTextBox;
            _toolStrip = toolStrip;

            AddToolStripItems();
            _richTextBox.TextChanged += OnTextChanged;

            // Add an event handler to the volume and page textboxes for when the user presses enter
            _volumeTextBox.KeyDown += new KeyEventHandler(HandleVolumeTextBoxKeyDown);
            _pageTextBox.KeyDown += new KeyEventHandler(HandlePageTextBoxKeyDown);
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            _richTextBox.TextChanged -= OnTextChanged; // Only do this once
            InitializeJournalText();
        }

        private void InitializeJournalText()
        {
            _journalText = new JournalText(_richTextBox, _cursorCoordinator);
            UpdateVolumeAndPageTextBoxes();
        }

        private void AddToolStripItems()
        {
            _volumeTextBox = new ToolStripTextBox();
            _pageTextBox = new ToolStripTextBox();
            _toolStrip.Items.Add(new ToolStripLabel("Volume:"));
            _toolStrip.Items.Add(_volumeTextBox);
            _toolStrip.Items.Add(new ToolStripLabel("Page:"));
            _toolStrip.Items.Add(_pageTextBox);
        }


        internal void UpdateVolumeAndPageTextBoxes()
        {
            if (_journalText == null) return;
            var result = _journalText.GetCurrentVolumeAndPage();
            _volumeTextBox.Text = result.Item1.ToString();
            _pageTextBox.Text = result.Item2.ToString();
        }

        private void HandleVolumeTextBoxKeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                int volume;
                if (_journalText != null && int.TryParse(_volumeTextBox.Text, out volume))
                {
                    _journalText.NavigateToPage(volume, 1);
                   
                }
            }
        }

        private void HandlePageTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int volume = int.Parse(_volumeTextBox.Text);
                int page = int.Parse(_pageTextBox.Text);
                NavigateToPage(volume, page);                
            }
        }

        private void NavigateToPage(int volume, int page) => _journalText?.NavigateToPage(volume, page);

        public void LoadFromIndexes(IIndexData indexData)
        {
            // Load the original text with page numbers so that can be parsed (pages should get removed by OnTextChanged)
            _richTextBox.Text = indexData.Text;
            // Next, load the Rtf version of the text without page numbers to speed up highlighting on load
            _richTextBox.Rtf = indexData.Rtf;
        }

        public void SaveToIndexes(IIndexData indexData)
        {
            indexData.Text = _journalText.OriginalText;
            indexData.Rtf = _richTextBox.Rtf;            
        }

        public void LoadFromProject(Project project) => _richTextBox.SelectionStart = project.CursorPosition;

        public void SaveToProject(Project project) => project.CursorPosition = _richTextBox.SelectionStart;
    }

}
