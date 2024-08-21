using Journadex.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{

    public abstract class Indexer : IRichTextBoxDecorator 
    {
        internal protected WebBrowser WebBrowser { get; }

        private const string RangePageLinkPrefix = "rangePage";
        private bool _showCheckboxes;
        private IRangeSelectionDestinationProvider _rangeSelectionDestinationProvider;
        private int _scrollPosition;
        private static readonly RangeComparer _rangeComparer = new RangeComparer();
        public event EventHandler CheckBoxSelectionChanged;
        protected int RangePage { get; set; } = 1;
        protected virtual int RangesPerPage => 0;


        public Func<Range, Tuple<string, string>> GetIndexRangeDescription { get; set; }
        protected RichTextBox RichTextBox { get; }
        public bool ShowCheckboxes
        {
            get => _showCheckboxes; set
            {
                bool changed = _showCheckboxes != value;
                _showCheckboxes = value;
                if (changed)
                {
                    if (ShowCheckboxes)
                    {
                        if (!(this is ICheckBoxSelectionOwner selectionOwner))
                        {
                            throw new NotSupportedException($"{nameof(ShowCheckboxes)} is not supported for {this}.");                            
                        }


                        CheckBoxSelection = new CheckBoxSelection(selectionOwner);
                        CheckBoxSelection.SelectionChanged += CheckBoxSelection_SelectionChanged;
                        WebBrowser.ObjectForScripting = CheckBoxSelection;
                    }
                    else
                    {
                        if (CheckBoxSelection != null)
                        {
                            CheckBoxSelection.SelectionChanged -= CheckBoxSelection_SelectionChanged;
                        }
                        CheckBoxSelection = null;
                        WebBrowser.ObjectForScripting = null;
                    }
                }
            }
        }

        private void CheckBoxSelection_SelectionChanged(object sender, EventArgs e) => CheckBoxSelectionChanged?.Invoke(this, e);

        public IRangeSelectionDestinationProvider RangeSelectionDestinationProvider
        {
            get { return _rangeSelectionDestinationProvider; }
            set
            {
                _rangeSelectionDestinationProvider = value;
                CheckBoxSelection.DestinationProvider = _rangeSelectionDestinationProvider;

            }
        }
        protected CheckBoxSelection CheckBoxSelection { get; private set; }

        //public Range[] SelectedRanges => RangeSelectionDestinationProvider?.SelectedItems;

        RichTextBox IRichTextBoxDecorator.RichTextBox => RichTextBox;
        internal CursorCoordinator CursorCoordinator { get; set; }



        public Indexer(RichTextBox richTextBox, WebBrowser webBrowser)
        {
            RichTextBox = richTextBox;
            WebBrowser = webBrowser;
            WebBrowser.Navigating += WebBrowser_Navigating;
            WebBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;

        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // Restore the scroll position after the content has been updated
            SetWebBrowserScrollPosition(WebBrowser, _scrollPosition);
        }


        // Handles clicks on links in the WebBrowser control
        private void WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            // Check if the link is an anchor link to a specific index
            if (e.Url.Fragment.Length > 0 && e.Url.Fragment[0] == '#')
            {
                ParseUrlFragment(e.Url.Fragment.Substring(1));

                // Cancel the navigation
                e.Cancel = true;
            }

        }

        /// <summary>
        /// By default, parses the range string from the fragment and selects that range in the RichTextBox.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void ParseUrlFragment(string fragment)
        {
            if (fragment.IndexOf(RangePageLinkPrefix) == 0)
            {
                RangePage = int.Parse(fragment.Replace(RangePageLinkPrefix, ""));
                UpdateIndex();
                return;
            }
            // Get the range from the anchor link
            Range range = Range.Parse(fragment);
            // Select the keyword in the RichTextBox
            CursorCoordinator.SetCursor(range.Start, range.Length);
            RichTextBox.ScrollToCaret();
        }

        /// <summary>
        /// Updates the WebBrowser control to display the current index        
        /// </summary>
        internal void UpdateIndex()
        {
            ProgressManager.Show(WebBrowser);
            try
            {
                // Save the current scroll position
                _scrollPosition = GetWebBrowserScrollPosition(WebBrowser);

                StringBuilder html = new StringBuilder();
                html.Append("<html>");
                BeforeHtmlBody(html);
                html.Append("<body>");
                BuildHtmlBody(html);

                html.Append("</body></html>");
                WebBrowser.SetDocumentTextSafe(html.ToString());
              
            }
            finally
            {
                ProgressManager.Hide();
            }
        }

        private int GetWebBrowserScrollPosition(WebBrowser webBrowser)
        {
            return webBrowser.Document?.Body != null
                ? webBrowser.Document.Body.ScrollTop
                : 0;
        }

        private void SetWebBrowserScrollPosition(WebBrowser webBrowser, int scrollPosition)
        {
            if (webBrowser.Document?.Body != null)
            {
                webBrowser.Document.Body.ScrollTop = scrollPosition;
            }
        }


        protected virtual void BeforeHtmlBody(StringBuilder html)
        {

        }

        internal abstract Dictionary<string, List<Range>> GetIndex();

        /// <summary>
        /// The method for building the HTML. Should call RenderRanges.
        /// </summary>
        /// <param name="html"></param>
        protected abstract void BuildHtmlBody(StringBuilder html);

        protected void RenderRanges(StringBuilder html, List<Range> indices)
        {
            // ChatGPT: Please add pagination to this C# method
            // > can you make the pageSize and pageNumber parameters optional?
            // > I want the pagination to only be included if the pageSize is specified.
            int pageSize = RangesPerPage;
            int pageNumber = RangePage;
            int startIndex = 0;
            int endIndex = indices.Count;

            if (pageSize > 0)
            {
                startIndex = (pageNumber - 1) * pageSize;
                endIndex = Math.Min(startIndex + pageSize, indices.Count);
            }
            AddPaginationLinks(html, indices, pageSize, pageNumber);
            html.Append("<ul>");
            // Eliminate duplicate ranges in the index - this causes the page to not render when using checkboxes
            foreach (var index in indices.Distinct(_rangeComparer).Skip(startIndex).Take(endIndex - startIndex))
            {
                if (index == null) continue;
                html.Append("<li>");
                RenderCheckBoxForRange(html, index);
                RenderRange(html, index);
                html.Append("</li>");
            }

            html.Append("</ul>");

            AddPaginationLinks(html, indices, pageSize, pageNumber);
        }

        private static void AddPaginationLinks(StringBuilder html, List<Range> indices, int pageSize, int pageNumber)
        {
            // Add pagination links if pageSize is specified
            if (pageSize > 0)
            {
                int totalPages = (int)Math.Ceiling(indices.Count / (double)pageSize);
                html.Append("<div class='pagination'>");
                for (int i = 1; i <= totalPages; i++)
                {
                    if (i == pageNumber)
                    {
                        html.Append($"<span class='current'>Page {i} </span>");
                    }
                    else
                    {
                        // TODO links don't work 
                        html.Append($"<a href='#{RangePageLinkPrefix}{i}'>Page {i}</a> ");
                    }
                }
                html.Append("</div>");
            }
        }

        protected void RenderCheckBoxForRange(StringBuilder html, Range range)
        {
            if (ShowCheckboxes)
            {
                var id = range.Id;//CheckBoxSelection.GetOrAddId(range);
                html.Append("<input type=\"checkbox\" id=\"checkbox_").Append(id).Append("\"");

                if (RangeSelectionDestinationProvider.IsSelected(id))
                {
                    html.Append(" checked");
                }
                html.Append(" onclick=\"window.external.OnCheckClicked('").Append(id).Append("')\" />");

            }
        }

        protected virtual void RenderRange(StringBuilder html, Range index) => RenderRange(html, index, GetIndexRangeDescription);

        public static void RenderRange(StringBuilder html, Range index, Func<Range, Tuple<string, string>> getIndexRangeDescription)
        {
            html.Append("<a href='#").Append(index.ToRangeString(useLength: true)).Append("'>");
            if (getIndexRangeDescription?.Invoke(index) is Tuple<string, string> description)
            {

                html.Append(description.Item1).Append("</a>");
                if (!string.IsNullOrEmpty(description.Item2))
                {
                    html.Append(description.Item2);
                }
            }
            else
            {
                html.Append(index.ToRangeString(useLength: true)).Append("</a>");
            }
        }


        internal void AddSelectedRanges(Range[] ranges)
        {
            if (ranges.Length == 0) return;
            if (!(this is ICheckBoxSelectionOwner selectionOwner))
            {
                throw new NotSupportedException($"{nameof(AddSelectedRanges)} is not supported for {this}.");
            }
            RangeSelectionDestinationProvider.AddSelectedItems(ranges, selectionOwner);
            UpdateIndex();
        }

        internal virtual void ScrollToRangeInBrowser(Range selectedRange)
        {
            var doc = WebBrowser.Document;
            var elements = doc.GetElementsByTagName("a");
            foreach (HtmlElement element in elements)
            {
                var rangeString = element.GetAttribute("href").Replace("about:blank#", "");
                if (Range.TryParse(rangeString, out var range) && range.Start <= selectedRange.Start && range.End >= selectedRange.Start)
                {
                    element.Focus();
                    element.ScrollIntoView(true);
                    return;
                }
            }
            //throw new InvalidOperationException($"No {this} element was found at the specified position.");
        }

        /// <summary>
        /// ChatGPT implementation 
        /// </summary>
        private class RangeComparer : IEqualityComparer<Range>
        {
            public bool Equals(Range x, Range y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;
                return x.Start == y.Start && x.End == y.End;
            }

            /// <summary>
            /// Check if the range is null and return 0 if it is. If it's not null, use the XOR operator to combine the hash codes of the start and end values of the range to return a unique hash code for that range.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(Range obj)
            {
                if (obj == null) return 0;
                return obj.Start.GetHashCode() ^ obj.End.GetHashCode();
            }
        }

    }
}