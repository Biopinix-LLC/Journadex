using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    /// <summary>
    /// ChatGPT: Create a class that shows and hides a docked find panel above a WinForms richtextbox control. The panel should provide a UI for the RichTextBox.Find method.
    /// > update the FindPanel to have a Find Previous and use the HistoryComponent to navigate.
    /// </summary>
    public class FindPanel : Panel
    {
        private readonly RichTextBox _target;
        private readonly TextBox _txtFind;
        private readonly CheckBox _chkMatchCase;
        private readonly CheckBox _chkWholeWord;
        private readonly Button _btnFindNext;
        private readonly Button _btnFindPrevious;
        private readonly HistoryComponent _history;

        public FindPanel(RichTextBox target)
        {
            Dock = DockStyle.Top;
            Padding = new Padding(3);
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowOnly;
            _target = target;

            _txtFind = new TextBox();
            _chkMatchCase = new CheckBox { Text = "&Match Case"};
            _chkWholeWord = new CheckBox { Text = "&Whole Word" };
            _btnFindNext = new Button { Text = "&Next" };
            _btnFindPrevious = new Button { Text = "&Previous"};

            _txtFind.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            _chkMatchCase.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            _btnFindNext.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            _btnFindPrevious.Anchor = AnchorStyles.Right | AnchorStyles.Top;


            _btnFindNext.Click += BtnFindNext_Click;
            _btnFindPrevious.Click += BtnFindPrevious_Click;

            DockControlsToTop(this, _txtFind, _chkMatchCase, _chkWholeWord, _btnFindNext, _btnFindPrevious);

            _history = new HistoryComponent(_target);
            _target.KeyDown += OnTargetKeyDown;
        }

        /// <summary>
        /// ChatGPT: pressing Ctrl+F in the richtextbox should display the panel and populate _txtFind with the currently selected text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTargetKeyDown(object sender, KeyEventArgs e)
        {
            // Check if the user pressed Ctrl+F
            if (e.Control && e.KeyCode == Keys.F)
            {
                ShowPanel();
                e.SuppressKeyPress = true;
            }
        }

        public static FindPanel Show(RichTextBox _target)
        {
            // Show the find panel and populate _txtFind with the currently selected text
            FindPanel panel = new FindPanel(_target);
            panel.ShowPanel();
            return panel;
        }

        public void ShowPanel()
        {
            if (!_target.Parent.Controls.Contains(this))
            {
                _target.Parent.Controls.Add(this);
                BringToFront();
            }
            _target.BringToFront();
            Visible = true;
            _txtFind.Text = _target.SelectedText;
            _txtFind.Focus();
            _txtFind.SelectAll();
        }

        private void BtnFindNext_Click(object sender, EventArgs e)
        {
            Find(_target.SelectionStart + _target.SelectionLength + 1, _txtFind.Text.Length, false);
        }

        private void BtnFindPrevious_Click(object sender, EventArgs e)
        {
            Find(_target.SelectionStart - 1, _txtFind.Text.Length, true);
        }

        private void Find(int startIndex, int length, bool backwards)
        {
            if (startIndex >= 0)
            {
                RichTextBoxFinds options = RichTextBoxFinds.None;
                if (_chkMatchCase.Checked)
                    options |= RichTextBoxFinds.MatchCase;
                if (_chkWholeWord.Checked)
                    options |= RichTextBoxFinds.WholeWord;
                if (backwards)
                    options |= RichTextBoxFinds.Reverse;

                int index = _target.Find(_txtFind.Text, startIndex, _target.TextLength, options);

                if (index >= 0)
                {
                    _target.Select(index, length);
                    _target.ScrollToCaret();
                }
                else
                {
                    MessageBox.Show("No more matches found.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _target.Select(startIndex, 0);
                }
            }
            else
            {
                MessageBox.Show("No matches found.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// ChatGPT: write a method that will take a params Control[] parameter and set each control to Dock Top and add them to a specified control so that they appear in the same order they are in for the list.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="controls"></param>
        public void DockControlsToTop(Control parent, params Control[] controls)
        {
            foreach (Control control in controls.Reverse())
            {
                control.Dock = DockStyle.Top;
                parent.Controls.Add(control);
                control.KeyDown += Control_KeyDown;
            }
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && Visible)
            {
                Visible = false;
                e.Handled = true;
                return;
            }
            if (sender == _btnFindNext) return;
            if (sender == _btnFindPrevious) return;
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                _btnFindNext.PerformClick();
                
            }
        }
    }



}
