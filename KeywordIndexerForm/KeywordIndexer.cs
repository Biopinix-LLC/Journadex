using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeywordIndex.WinForms
{
    public class KeywordIndexer
    {
        private RichTextBox _richTextBox;
        private WebBrowser _webBrowser;
        private Dictionary<string, List<int>> _index;

        public KeywordIndexer(RichTextBox richTextBox, WebBrowser webBrowser)
        {
            _richTextBox = richTextBox;
            _webBrowser = webBrowser;
            _index = new Dictionary<string, List<int>>();

            // Set up event handlers for adding items to the index
            _richTextBox.MouseUp += AddToIndex;
            _richTextBox.KeyUp += AddToIndex;
        }

        // Event handler for adding items to the index
        private void AddToIndex(object sender, EventArgs e)
        {
            if (e is MouseEventArgs mouseEvent)
            {
                // Check if the right mouse button was clicked
                if (mouseEvent.Button == MouseButtons.Right)
                {
                    AddSelectedToIndex();
                }
            }
            else if (e is KeyEventArgs keyEvent)
            {
                // Check if the "I" key was pressed
                if (keyEvent.Control && keyEvent.KeyCode == Keys.I)
                {
                    AddSelectedToIndex();
                }
            }
        }

        // Adds the currently selected text in the RichTextBox to the index
        private void AddSelectedToIndex()
        {
            if (_richTextBox.SelectedText.Length > 0)
            {
                string keyword = _richTextBox.SelectedText;
                int startIndex = _richTextBox.SelectionStart;

                if (_index.ContainsKey(keyword))
                {
                    _index[keyword].Add(startIndex);
                }
                else
                {
                    _index[keyword] = new List<int> { startIndex };
                }

                UpdateIndex();
            }
        }

        // Updates the WebBrowser control to display the current index
        private void UpdateIndex()
        {
            StringBuilder html = new StringBuilder();
            html.Append("<html><body>");

            foreach (var entry in _index)
            {
                string keyword = entry.Key;
                List<int> indices = entry.Value;

                html.Append($"<h2>{keyword}</h2>");
                html.Append("<ul>");

                foreach (int index in indices)
                {
                    html.Append($"<li><a href='#{index}'>{index}</a></li>");
                }

                html.Append("</ul>");
            }

            html.Append("</body></html>");
            _webBrowser.DocumentText = html.ToString();
        }

        // Event that is raised when a link in the index is clicked
        public event EventHandler<IndexClickedEventArgs> IndexClicked;

        // Raises the IndexClicked event
        private void OnIndexClicked(int startIndex, int endIndex)
        {
            IndexClicked?.Invoke(this, new IndexClickedEventArgs(startIndex, endIndex));
        }


        // Handles clicks on links in the WebBrowser control
        private void WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            // Check if the link is an anchor link to a specific index
            if (e.Url.Fragment.Length > 0)
            {
                // Get the index from the anchor link
                int index = int.Parse(e.Url.Fragment.Substring(1));

                // Find the keyword associated with the index
                string keyword = _index.FirstOrDefault(x => x.Value.Contains(index)).Key;

                // Select the keyword in the RichTextBox
                _richTextBox.SelectionStart = index;
                _richTextBox.SelectionLength = keyword.Length;
                _richTextBox.ScrollToCaret();

                // Raise the IndexClicked event
                OnIndexClicked(index, index + keyword.Length);
            }

            // Cancel the navigation
            e.Cancel = true;
        }
    }

    // Event args for the IndexClicked event
    public class IndexClickedEventArgs : EventArgs
    {
        public int StartIndex { get; }
        public int EndIndex { get; }

        public IndexClickedEventArgs(int startIndex, int endIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }
    }
}
