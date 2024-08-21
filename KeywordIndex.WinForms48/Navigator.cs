using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Journadex.Library;
using System.Linq;

namespace KeywordIndex.WinForms48
{

    /// <summary>
    /// ChatGPT: Create a C# class that adds next (▶) and previous (◀) buttons and an autocomplete combo box to an existing WinForms toolstrip. The combobox list should display a list of keywords. Each keyword added to list has a list of Range objects. Range is a class with Start and End integers referencing the location of the keyword within the text of a specified RichTextBox. Clicking the next and previous buttons should navigate through the list for the selected keyword and select the range in the rich text box.
    /// </summary>
    internal class Navigator : IRichTextBoxDecorator, ISelectKeyword
    {
        private ToolStripButton nextButton;
        private ToolStripButton prevButton;
        private ToolStripButton firstButton;
        private ToolStripComboBox keywordsComboBox;

        private Dictionary<string, List<Range>> keywordRanges;
        private string selectedKeyword;
        private int selectedRangeIndex;
        private ToolStripButton lastButton;

        public RichTextBox RichTextBox { get; }
        internal CursorCoordinator CursorCoordinator { get; set; }

        public Navigator(ToolStrip toolStrip, RichTextBox richTextBox)
        {
            RichTextBox = richTextBox;

            keywordRanges = new Dictionary<string, List<Range>>(StringComparer.InvariantCultureIgnoreCase);

            nextButton = new ToolStripButton
            {
                Text = "▶",
                ToolTipText = "Next"
            };
            nextButton.Click += NextButton_Click;

            prevButton = new ToolStripButton
            {
                Text = "◀",
                ToolTipText = "Previous"
            };
            prevButton.Click += PreviousButton_Click;

            firstButton = new ToolStripButton
            {
                Text = "|◀",
                ToolTipText = "First"
            };
            firstButton.Click += FirstButton_Click;

            lastButton = new ToolStripButton
            {
                Text = "▶|",
                ToolTipText = "Last"
            };
            lastButton.Click += LastButton_Click;

            keywordsComboBox = new ToolStripComboBox
            {
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems,
                Sorted = true
            };
            keywordsComboBox.SelectedIndexChanged += KeywordsComboBox_SelectedIndexChanged;

            #region ChatGPT help
            // Me > I'm handling the keyup event of a winforms toolstrip combobox when the Alt key is down and Left, Right, Home or End are pressed. Left and Right works fine but Home and End  select the first or last button in the toolstrip instead of following my logic.
            // Chat GPT > The Home and End keys are likely being handled by the operating system or the ToolStripComboBox control itself, rather than your event handler.To handle these keys yourself, you may need to set the KeyPreview property of your Form to true, and then handle the KeyUp event of the Form instead of the ToolStripComboBox.This will allow your event handler to receive the key presses before they are passed on to the control or the operating system.
            Form form = toolStrip.FindForm();
            form.KeyPreview = true;
            form.KeyUp += KeywordsComboBox_KeyUp;
            #endregion

            toolStrip.Items.AddRange(new ToolStripItem[] { firstButton, prevButton, keywordsComboBox, nextButton, lastButton });
        }
        private void KeywordsComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                keywordsComboBox.Focus();
                return;
            }
            if (!e.Alt) return;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    GoToPrevious();
                    break;
                case Keys.Right:
                    GoToNext();
                    break;
                case Keys.Home:
                    GoToFirst();
                    break;
                case Keys.End:
                    GoToLast();
                    break;

            }
        }

        private void LastButton_Click(object sender, EventArgs e) => GoToLast();

        private void GoToLast()
        {
            if (selectedKeyword != null && keywordRanges[selectedKeyword].Count > 0)
            {
                selectedRangeIndex = keywordRanges[selectedKeyword].Count - 1;
                SelectRange(keywordRanges[selectedKeyword][selectedRangeIndex]);
            }
        }

        private void FirstButton_Click(object sender, EventArgs e) => GoToFirst();

        private void GoToFirst()
        {
            if (selectedKeyword != null && keywordRanges[selectedKeyword].Count > 0)
            {
                selectedRangeIndex = 0;
                SelectRange(keywordRanges[selectedKeyword][selectedRangeIndex]);
            }
        }

        private void NextButton_Click(object sender, EventArgs e) => GoToNext();

        private void GoToNext()
        {
            if (selectedKeyword != null && keywordRanges[selectedKeyword].Count > 0)
            {
                selectedRangeIndex = (selectedRangeIndex + 1) % keywordRanges[selectedKeyword].Count;
                SelectRange(keywordRanges[selectedKeyword][selectedRangeIndex]);
            }
        }

        private void PreviousButton_Click(object sender, EventArgs e) => GoToPrevious();

        private void GoToPrevious()
        {
            if (selectedKeyword != null && keywordRanges[selectedKeyword].Count > 0)
            {
                selectedRangeIndex = (selectedRangeIndex - 1 + keywordRanges[selectedKeyword].Count) % keywordRanges[selectedKeyword].Count;
                SelectRange(keywordRanges[selectedKeyword][selectedRangeIndex]);
            }
        }

        private void KeywordsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {            
            selectedKeyword = keywordsComboBox.SelectedItem as string;
            // When the History item is selected, should stay on current and not go to first
            if (selectedKeyword == HistoryComponent.IndexKey) return;

            selectedRangeIndex = 0;

            if (selectedKeyword != null)
            {
                SelectRange(keywordRanges[selectedKeyword][selectedRangeIndex]);
            }
        }

        private void SelectRange(Range range)
        {
            if (range == null) return;
            CursorCoordinator.SetCursor(range.Start, range.Length);
        }

        internal void AddKeyword(string keyword, Range range)
        {
            keyword = keyword.ToTitleCase();
            if (!keywordRanges.ContainsKey(keyword))
            {
                keywordRanges[keyword] = new List<Range>();
                keywordsComboBox.Items.Add(keyword);
            }

            keywordRanges[keyword].Add(range);
        }

        internal void RemoveKeyword(string indexKey, Range range)
        {
            if (string.IsNullOrEmpty(indexKey)) throw new ArgumentNullException(nameof(indexKey));
            if (!keywordRanges.ContainsKey(indexKey)) throw new KeyNotFoundException();

            List<Range> ranges = keywordRanges[indexKey];
            if (range != null)
            {
                ranges.Remove(range);
            }

            if (range == null || ranges.Count == 0)
            {
                keywordRanges.Remove(indexKey);
                keywordsComboBox.Items.Remove(indexKey.ToTitleCase());
            }
        }

        internal void AddFromRangeList(IRangeListContainer rangeList)
        {
            foreach (var (kvp, range) in from kvp in rangeList.GetIndex()
                                         from range in kvp.Value
                                         select (kvp, range))
            {
                AddKeyword(kvp.Key, range);
            }
        }
        internal void Select(string indexKey, Range range = null)
        {
            keywordsComboBox.SelectedItem = indexKey;
            if (range != null)
            {
                SetSelectedRangeIndexNearestTo(range);
            }
        }

        public void SelectKeyword(string keyword, Range range = null) => Select(keyword, range);

        internal void SetSelectedRangeIndexNearestTo(Range selectedRange)
        {
            int index = keywordRanges.GetRangeIndexContaining(selectedRange.Start);
            if (index == -1)
            {
                index = 0;
            }
            else if (index >= keywordRanges.Count)
            {
                index = keywordRanges.Count - 1;
            }
            selectedRangeIndex = index;
        }

        
    }
}
