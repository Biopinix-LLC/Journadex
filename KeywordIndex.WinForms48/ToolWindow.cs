using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    public partial class ToolWindow : Form
    {
        private RichTextBox _richTextBox;
        private IEnumerable<string> _aliasSource;
        private ToolStrip _toolStrip;
        private ToolStripButton _addButton;
        private ToolStripButton _deleteButton;
        private ToolStripComboBox _aliasComboBox;
        private bool _selectionChanged;

        public event EventHandler AddButtonClicked;
        public event EventHandler DeleteButtonClicked;
        public event EventHandler<SelectedStringArgs> AliasSelectionChanged; 
        public ToolWindow()
        {
            InitializeComponent();
        }

        public ToolWindow(RichTextBox richTextBox, IEnumerable<string> aliasSource)
        {
            InitializeComponent();
            _richTextBox = richTextBox;
            _aliasSource = aliasSource;
            _toolStrip = new ToolStrip();
            _toolStrip.SuspendLayout();
            _addButton = new ToolStripButton { Text = "Add" };
            _deleteButton = new ToolStripButton { Text = "Delete" };
            _aliasComboBox = new ToolStripComboBox { Text = "Alias" };
            _aliasComboBox.Enter += AliasComboBox_Enter;
            _addButton.Click += AddButton_Click;
            _deleteButton.Click += DeleteButton_Click;
            _aliasComboBox.SelectedIndexChanged += AliasComboBox_SelectedIndexChanged;
            _toolStrip.Items.AddRange(new ToolStripItem[] { _addButton, _deleteButton, _aliasComboBox });
            _toolStrip.Width = _addButton.Width + _deleteButton.Width + _aliasComboBox.Width;
            _toolStrip.Height = _addButton.Height + _deleteButton.Height + _aliasComboBox.Height;
            _toolStrip.ResumeLayout();    
            Width = _toolStrip.Width;
            Height= _toolStrip.Height;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            TopLevel = true;
            AutoSize = false;
            Controls.Add(_toolStrip);
            _richTextBox.SelectionChanged += RichTextBox_SelectionChanged;
            _richTextBox.MouseUp += RichTextBox_MouseUp;


        }

        private void RichTextBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (_selectionChanged)
            {
                _selectionChanged = false;
                AfterSelectionChanged();
            }
        }

        private void AliasComboBox_Enter(object sender, EventArgs e)
        {
            _aliasComboBox.Items.Clear();
            _aliasComboBox.Items.AddRange(_aliasSource.ToArray());
        }

        private void RichTextBox_SelectionChanged(object sender, EventArgs e)
        {
            // TODO resize properly
            // TODO don't hide so quickly
            _selectionChanged = true;
        }

        private void AfterSelectionChanged()
        {
            if (Visible)
            {
                if (_richTextBox.SelectionLength <= 1)
                {
                    Hide();
                }
            }
            else
            {
                if (_richTextBox.SelectionLength > 1)
                {
                    Show();
                }
            }
            if (Visible)
            {
                PositionFormNextToSelection(this, _richTextBox);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
            base.OnClosing(e);
        }
        private static void PositionFormNextToSelection(Form form, RichTextBox richTextBox)
        {
            // Get the position of the current selection in the RichTextBox
            Point selectionPosition = richTextBox.GetPositionFromCharIndex(richTextBox.SelectionStart);

            // Set the position of the form to be just to the right of the current selection
            form.Left = richTextBox.Left + selectionPosition.X + richTextBox.Font.Height;
            form.Top = richTextBox.Top + selectionPosition.Y;
        }

        private void AddButton_Click(object sender, EventArgs e) => OnAddButtonClicked(EventArgs.Empty);

        private void OnAddButtonClicked(EventArgs args) => AddButtonClicked?.Invoke(this, args);

        private void DeleteButton_Click(object sender, EventArgs e)
        => OnDeleteButtonClicked(e);

        private void OnDeleteButtonClicked(EventArgs args)
        => DeleteButtonClicked?.Invoke(this, args);

        private void AliasComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnAliasSelectionChanged(new SelectedStringArgs { SelectedString = _aliasComboBox.SelectedItem as string });
        }

        private void OnAliasSelectionChanged(SelectedStringArgs selectedStringArgs) => AliasSelectionChanged?.Invoke(this, selectedStringArgs);
    }
}
