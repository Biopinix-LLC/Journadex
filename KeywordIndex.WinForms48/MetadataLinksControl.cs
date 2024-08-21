using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    public partial class MetadataLinksControl : UserControl
    {
        private Label _titleLabel;
        private ListBox _metadataListBox;
        private TextBox _addMetadataTextBox;
        private Button _addButton;
        private Button _removeButton;
        private LinkLabel _urlLinkLabel;

        public MetadataLinksControl(string keyword, List<string> metadata)
        {
            InitializeComponents();
            _titleLabel.Text = keyword;
            _metadataListBox.DataSource = metadata;
        }

        private void InitializeComponents()
        {
            _titleLabel = new Label { Location = new Point(10, 10), AutoSize = true };
            _metadataListBox = new ListBox { Location = new Point(10, 30), Size = new Size(260, 100) };
            _addMetadataTextBox = new TextBox { Location = new Point(10, 140), Size = new Size(260, 20) };
            _addButton = new Button { Text = "Add", Location = new Point(10, 170) };
            _removeButton = new Button { Text = "Remove", Location = new Point(100, 170) };
            _urlLinkLabel = new LinkLabel { Location = new Point(10, 200), AutoSize = true };

            _addButton.Click += AddButton_Click;
            _removeButton.Click += RemoveButton_Click;
            _metadataListBox.SelectedIndexChanged += MetadataListBox_SelectedIndexChanged;
            _urlLinkLabel.LinkClicked += UrlLinkLabel_LinkClicked;

            Controls.AddRange(new Control[] { _titleLabel, _metadataListBox, _addMetadataTextBox, _addButton, _removeButton, _urlLinkLabel });


        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_addMetadataTextBox.Text))
            {
                (_metadataListBox.DataSource as List<string>).Add(_addMetadataTextBox.Text);
                _metadataListBox.Refresh();
                _addMetadataTextBox.Clear();
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (_metadataListBox.SelectedItem is string selectedString)
            {
                (_metadataListBox.DataSource as List<string>).Remove(selectedString);
            }
        }

        private void MetadataListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_metadataListBox.SelectedItem != null)
            {
                string selectedItem = _metadataListBox.SelectedItem.ToString();
                _urlLinkLabel.Text = Uri.IsWellFormedUriString(selectedItem, UriKind.Absolute) ? selectedItem : string.Empty;
            }
        }

        private void UrlLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_urlLinkLabel.Text))
            {
                Process.Start(new ProcessStartInfo(_urlLinkLabel.Text) { UseShellExecute = true });
            }
        }
    }
}
