using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Journadex.Library;
namespace KeywordIndex.WinForms48
{
    public partial class KeywordIndexer : Indexer, ISelectKeyword, IIndexFileComponent, ICheckBoxSelectionOwner, IRangeListContainer
    {
        private const string PageLinkPrefix = "page";
        private const int ItemsPerPage = 1;
        private const string TagLinkPrefix = "tag_";
        private const string MetadataLinkPrefix = "metadata_";
        private const string MetadataLinkLabel = "More";
        private RichTextBox _richTextBox;
        private KeywordIndexContextMenuHandler _contextMenu;

        public event EventHandler<RangeListChangedEventArgs> IndexChanged;
        private Dictionary<string, IndexItemInfo> Index { get; set; } = new Dictionary<string, IndexItemInfo>(StringComparer.CurrentCultureIgnoreCase);
        private Dictionary<string, string> Aliases { get; set; } = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

        public int ItemCount => Index.Count;

        public int CurrentPage { get; set; } = 1;

        public KeywordIndexer(RichTextBox richTextBox, WebBrowser webBrowser) : base(richTextBox, webBrowser)
        {
            _richTextBox = richTextBox;
            _richTextBox.KeyUp += RichTextBox_KeyUp;
            _contextMenu = new KeywordIndexContextMenuHandler(webBrowser, this);

        }


        /// <summary>
        /// Event handler for adding items to the index
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RichTextBox_KeyUp(object sender, EventArgs e)
        {
            if (e is KeyEventArgs keyEvent)
            {
                // Check if the "I" key was pressed
                if (keyEvent.Control && keyEvent.KeyCode == Keys.I)
                {
                    AddSelectedToIndex();
                }
            }
        }

        internal void AddSelectedAsTagToIndex(string tag, bool toAllMatchingSelected, bool isTag = false, string parentName = null)
        {
            if (toAllMatchingSelected)
            {
                AddSelectedToIndex(tag, asTagForAll: true, isTag, parentName);
                return;
            }
            AddToIndex(tag, _richTextBox.GetSelectedRange(), isTag, parentName);
            UpdateIndex();
        }

        /// <summary>
        /// Adds the currently selected text in the RichTextBox to the index
        /// </summary>
        /// <param name="indexKey">The key of the index to add the selection range to</param>
        /// <param name="asTagForAll">IndexItemInfo all occurances of the selected text with the index key</param>
        internal void AddSelectedToIndex(string indexKey = null, bool asTagForAll = false, bool isTag = false, string parentName = null)
        {
            if (_richTextBox.SelectedText.Length == 0) return;
            string keyword = _richTextBox.SelectedText;
            indexKey = indexKey ?? keyword;
            int startIndex = 0;
            int count = 0;
            int originalPosition = _richTextBox.SelectionStart;
            // Find all occurrences of the selected text
            while (startIndex < _richTextBox.TextLength)
            {
                startIndex = _richTextBox.Find(keyword, startIndex, RichTextBoxFinds.None);

                if (startIndex == -1)
                {
                    // No more occurrences found, break out of the loop
                    break;
                }
                count++;
                // Add the occurrence to the index
                Range range = new Range(startIndex, Range.GetEndFromLength(startIndex, keyword.Length));
                AddToIndex(indexKey, range, isTag, parentName);

                // Continue searching after the current occurrence
                startIndex += keyword.Length;
            }
            _richTextBox.SelectionStart = originalPosition;
            UpdateIndex();
            MessagePanel.Show(_richTextBox,$"{count} occurrences of the selected text were added."); 
        }

        private IndexItemInfo AddToIndex(string indexKey, Range range = null, bool isTag = false, string parentName = null)
        {
            if (indexKey == DateIndexer.EntriesKey || indexKey == DateIndexer.DatesKey || indexKey == HistoryComponent.IndexKey) throw new InvalidOperationException($"{indexKey} is reserved for internal use.");
            range = range != null ? _richTextBox.TrimRange(range) : null;
            indexKey = indexKey.Trim();
            bool addRange = true;
            var parentTag = !string.IsNullOrEmpty(parentName) ? GetOrAddItem(parentName) : null;

            if (Index.TryGetValue(indexKey, out IndexItemInfo info))
            {
                // Don't add the range if the range already exists. Add as a tag instead.
                if (range == null || info.Ranges.Any(r => r.Start == range.Start && r.End == range.End))
                {
                    addRange = false;
                    // TODO do we need to add it if it already exists? How?
                }
                

                if (addRange)
                {
                    info.Add(range);
                }
            }
            else
            {
                info = new IndexItemInfo(indexKey, range, isTag);
                if (parentTag != null)
                {
                    info.SetParent(parentTag);
                }
                Index[indexKey] = info;
                
            }

            if (range != null)
            {
                HighlightRange(range, isTag);
            }

            OnIndexChanged(new RangeListChangedEventArgs(ListChangedType.ItemAdded, indexKey, range));
            return info;
        }


        private void OnIndexChanged(RangeListChangedEventArgs args) => IndexChanged?.Invoke(this, args);

        private void HighlightRange(Range range, bool isTag) => _richTextBox.SetBackgroundColor(range, isTag ? Color.LightBlue : Color.Yellow);
        private void RemoveHighlight(Range range) => _richTextBox.SetBackgroundColor(range, Control.DefaultBackColor);

        private string[] GetAliasesFor(string keyword)
        {
            return (from kvp in Aliases where kvp.Value.Equals(keyword, StringComparison.CurrentCultureIgnoreCase) select kvp.Key).ToArray();
        }

        protected override void ParseUrlFragment(string fragment)
        {
            if (fragment.IndexOf(MetadataLinkPrefix) == 0)
            {
                string keyword = fragment.Substring(MetadataLinkPrefix.Length).Replace("%20", " ");
                ShowMetadataLinksDialog(keyword);
                return;
            }
            if (fragment.IndexOf(TagLinkPrefix) == 0)
            {
                CurrentPage = GetPageWithTag(fragment.Replace(TagLinkPrefix, "").Replace("%20", " "));
                UpdateIndex();
                return;
            }
            else if (fragment.IndexOf(PageLinkPrefix) == 0)
            {
                CurrentPage = int.Parse(fragment.Replace(PageLinkPrefix, ""));
                UpdateIndex();
                return;
            }
            base.ParseUrlFragment(fragment);
        }

        private void ShowMetadataLinksDialog(string keyword)
        {
            List<string> metadata = Index[keyword].Metadata;
            using (MetadataLinksDialog dialog = new MetadataLinksDialog(keyword, metadata))
            {
                dialog.ShowDialog();
            }
        }

        private int GetPageWithTag(string tagName)
        {
            var items = Index.OrderBy(kvp => kvp.Key).ToList();
            int page = 1;
            for (int i = 0; i < items.Count; i++)
            {

                if (items[i].Key.Equals(tagName, StringComparison.OrdinalIgnoreCase))
                {
                    return page;
                }

                if ((i + 1) % ItemsPerPage == 0)
                {
                    page++;
                }
            }

            return -1;
        }

        internal override void ScrollToRangeInBrowser(Range selectedRange)
        {
            var (selectedPage, rangePage) = GetPageWithRange(selectedRange);
            if ((selectedPage > -1 && CurrentPage != selectedPage) || (rangePage > -1 && RangePage != rangePage))
            {
                CurrentPage = selectedPage;
                RangePage = rangePage;
                UpdateIndex();
            }
            base.ScrollToRangeInBrowser(selectedRange);
        }
        /// <summary>
        /// ChatGPT: I need a method that will get me the first page where the item with specified range is found.
        /// > I need to update the following C# method. I'm paging by Index items and now also by the ranges. So I need this to now also determine the range page in addition to the index page which it is already returning. There is a RangesPerPage integer property. 
        /// TODO to use this with all Indexers you would need to refactor out the rangePage code into a separate method (not needed right now)
        /// </summary>
        /// <param name="r"></param>
        /// <returns>(index page, range page)</returns>
        private (int, int) GetPageWithRange(Range r)
        {
            // TODO this is usually returning the first range page because when selecting the text the coordinator selects the keyword
            // which scrolls to the first range. It would be better to only select the first range if the keyword has changed.
            var items = Index.OrderBy(kvp => kvp.Key).ToList();
            int indexPage = 1;
            int rangePage = 1;

            for (int i = 0; i < items.Count; i++)
            {
                var ranges = items[i].Value.Ranges;
                for (int j = 0; j < ranges.Count; j++)
                {
                    Range range = ranges[j];
                    if (range.Contains(r))
                    {
                        return (indexPage, rangePage);
                    }
                    if ((j + 1) % RangesPerPage == 0)
                    {
                        rangePage++;
                    }
                }

                // Reset rangePage to 1 at the start of each new index page
                rangePage = 1;

                if ((i + 1) % ItemsPerPage == 0)
                {
                    indexPage++;
                }
            }

            return (-1, -1);
        }


        protected override int RangesPerPage => 20;

        bool ICheckBoxSelectionOwner.IsDateIndex => false;

        protected override void BuildHtmlBody(StringBuilder html)
        {
            if (Index.Count == 0) return;

            var items = Index.OrderBy(kvp => kvp.Key).ToList();
            int numberOfPages = (int)Math.Ceiling((double)items.Count / ItemsPerPage);

            int startIndex = (CurrentPage - 1) * ItemsPerPage;
            int endIndex = Math.Min(startIndex + ItemsPerPage - 1, items.Count - 1);

            if (CurrentPage > 1)
            {
                html.Append($"<a href='#{PageLinkPrefix}{CurrentPage - 1}'> <- {items[startIndex - ItemsPerPage].Key.ToTitleCase()}</a>");
            }

            if (CurrentPage < numberOfPages)
            {
                if (CurrentPage > 1) { html.Append(" | "); }
                html.Append($"<a href='#{PageLinkPrefix}{CurrentPage + 1}'>{items[endIndex + 1].Key.ToTitleCase()} -> </a><br>");
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                string keyword = items[i].Key;
                IndexItemInfo info = items[i].Value;
                var indices = info.Ranges;
                var aliases = GetAliasesFor(keyword).Select(s => s.ToTitleCase()).OrderBy(s => s).ToArray();
                if (info.Parent != null)
                {
                    // Only add another line if the break hasn't already been added
                    if (html.Length > 2 && html[html.Length - 2] != 'r')
                    {
                        html.Append("<br>");
                    }

                    RenderAncestors(html, info);
                    html.Append("<br>");
                }
                html.Append($"<h2 class='item' id='{PageLinkPrefix}{CurrentPage}'>{keyword.ToTitleCase()}</h2><br>");
                if (info.Children != null)
                {
                    RenderDescendants(html, info);
                }
                RenderMetadataLink(html, info);
                if (aliases.Length > 0)
                {
                    html.Append("<h3>Also known as </h3>");

                    for (int aliasIndex = 0; aliasIndex < aliases.Length; aliasIndex++)
                    {
                        string alias = aliases[aliasIndex];
                        html.Append("<span class='alias'>").Append(alias).Append("</span>");
                        if (aliasIndex < aliases.Length - 1)
                        {
                            html.Append(';');
                        }
                    }
                }
                // TODO improve performance
                RenderRanges(html, indices);
            }
        }



        private static StringBuilder RenderMetadataLink(StringBuilder html, IndexItemInfo info)
        {
            return html.Append("<a href='#").Append(MetadataLinkPrefix).Append(info.Keyword).Append("'>").Append(MetadataLinkLabel).Append("</a>");
        }

        /// <summary>
        /// ChatGPT: I need this method to render the html of the descendants of this item as a list that indents but doesn't show a bullet.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="info"></param>
        /// <param name="indentLevel"></param>
        private static void RenderDescendants(StringBuilder html, IndexItemInfo info, int indentLevel = 0)
        {
            foreach (var child in info.Children.OrderBy(c=>c.Keyword))
            {
                html.Append("<div style=\"padding-left: " + (indentLevel * 10) + "px;\">");
                RenderTagLink(html, child);
                html.Append("</div>");
                RenderDescendants(html, child, indentLevel + 1);
            }
        }

        /// <summary>
        /// ChatGPT: I need the same for this one... I need it to start at the root and go down... The root should not be indented and the indentation should increase as you go deeper.... that didn't work. I lost the indent on most levels and the root is still the most indented.... I think it might be better for RenderAncestors to find the root first and traverse down to the current IndexItemInfo. Can you try that? one more thing, RenderAncestors should not include the current item as a tag link, only the ancestors
        /// </summary>
        /// <param name="html"></param>
        /// <param name="info"></param>
        /// <param name="indentLevel"></param>
        private static void RenderAncestors(StringBuilder html, IndexItemInfo info)
        {
            var ancestors = new List<IndexItemInfo>();
            var currentItem = info;
            while (currentItem.Parent != null)
            {
                ancestors.Add(currentItem.Parent);
                currentItem = currentItem.Parent;
            }
            ancestors.Reverse(); // Reverse the list to start from the root

            foreach (var ancestor in ancestors)
            {
                html.Append("<div style=\"padding-left: " + (ancestors.IndexOf(ancestor) * 10) + "px;\">");
                RenderTagLink(html, ancestor);
                html.Append("</div>");
            }
        }




        private static StringBuilder RenderTagLink(StringBuilder html, IndexItemInfo tag)
        {
            return html.Append("<a href='#").Append(TagLinkPrefix).Append(tag.Keyword).Append("'>").Append(tag.Keyword).Append("</a>");
        }

        public bool Contains(string text) => Index.ContainsKey(text) || Aliases.ContainsKey(text);

        internal bool TryGetSelectedTextAsAlias(out string indexItem) => Aliases.TryGetValue(_richTextBox.SelectedText, out indexItem);

        /// <summary>
        /// Gets the Title-cased keywords in the index ordered alphabetically.
        /// </summary>
        /// <returns></returns>
        internal string[] GetKeys() => Index.Keys.Select(s => s.ToTitleCase()).OrderBy(s => s).ToArray();

        internal void RemoveSelectedFromIndex()
        {
            string selectedText = _richTextBox.SelectedText;
            if (Index.ContainsKey(selectedText))
            {
                Remove(selectedText);
            }
            else if (Aliases.TryGetValue(selectedText, out string indexKey))
            {
                RemoveAlias(selectedText, indexKey);
            }
        }

        private void RemoveAlias(string alias, string indexKey)
        {
            List<Range> ranges = Index[indexKey].Ranges;
            ranges.RemoveAll(r => MatchesText(r, alias));
            Aliases.Remove(alias);
            ranges.ForEach(r => OnRemoveRange(r, indexKey));
            UpdateIndex();
        }

        private void OnRemoveRange(Range r, string indexKey)
        {
            RemoveHighlight(r);
            OnIndexChanged(new RangeListChangedEventArgs(ListChangedType.ItemDeleted, indexKey, r));
        }

        private bool MatchesSelectedText(Range r) => MatchesText(r, _richTextBox.SelectedText);

        private bool MatchesText(Range r, string text) => r != null && _richTextBox.Text.Substring(r).Equals(text, StringComparison.OrdinalIgnoreCase);

        internal void AddSelectedAsAliasOf(string indexKey)
        {
            if (string.IsNullOrEmpty(indexKey))
            {
                throw new ArgumentException($"'{nameof(indexKey)}' cannot be null or empty.", nameof(indexKey));
            }

            Aliases.Add(_richTextBox.SelectedText, indexKey);
            AddSelectedToIndex(indexKey);
        }

        //internal string[] GetTags() => (from kvp in Index where kvp.Value.IsTag select kvp.Key).ToArray();
        internal Dictionary<string, Range> GetTagsForSelectedText()
        {
            var tagIndexItems = new Dictionary<string, Range>();

            foreach (var indexItem in Index)
            {
                if (indexItem.Value.IsTag)
                {
                    foreach (var range in indexItem.Value.Ranges)
                    {
                        if (range.Start >= _richTextBox.SelectionStart && range.End <= _richTextBox.SelectionStart + _richTextBox.SelectionLength)
                        {
                            tagIndexItems[indexItem.Key] = range;
                            break;
                        }
                    }
                }
            }

            return tagIndexItems;
        }

        internal void RemoveTagRange(string key, Range range, out int rangesCount)
        {
            IndexItemInfo indexItemInfo = Index[key];
            indexItemInfo.Ranges.Remove(range);
            rangesCount = indexItemInfo.Ranges.Count;
            UpdateIndex();
        }

        internal void Remove(string key)
        {
            if (!Index.TryGetValue(key, out var itemInfo)) return;
            if (itemInfo.Children?.Count > 0)
            {
                while (itemInfo.Children.Count > 0)
                {
                    itemInfo.Children.Last().SetParent(null);
                    // TODO SetParent should handle removing the children - not sure why it doesn't
                    itemInfo.Children.RemoveAt(itemInfo.Children.Count - 1);
                }
            }
            foreach (var range in itemInfo.Ranges)
            {
                if (range == null) continue;
                RemoveHighlight(range);
            }
            Index.Remove(key);
            OnIndexChanged(new RangeListChangedEventArgs(ListChangedType.ItemDeleted, key, null));
            UpdateIndex();

        }

        public void LoadFromIndexes(IIndexData indexData)
        {
            var info = indexData.Keywords;
            if (info == null) return;
            if (info.Aliases != null)
            {
                Aliases = ToCaseInsensitiveDictionary(info.Aliases);
            }
            
            if (info.Index != null)
            {
                info.Index.RemoveNullRanges();
                Index = ToCaseInsensitiveDictionary(info.Index);
                UpdateIndex();
            }
        }

        public void SaveToIndexes(IIndexData indexData)
        {
            indexData.Keywords = new KeywordInfo { Index = Index, Aliases = Aliases };
        }

        internal override Dictionary<string, List<Range>> GetIndex()
        {
            Dictionary<string, List<Range>> result = new Dictionary<string, List<Range>>();
            foreach (var kvp in Index)
            {

                result[kvp.Key] = FindRangesOfFirstChildItemWithRanges(kvp.Key).ToArray().ToList(); // TODO I'm hoping this creates new list objects so we don't have sycing issues

            }
            return result;
        }

        private List<Range> FindRangesOfFirstChildItemWithRanges(string key)
        {
            if (Index.TryGetValue(key, out var value))
            {
                if (value.Ranges.Count == 0)
                {
                    foreach (var tag in value.Children)
                    {
                        var result = FindRangesOfFirstChildItemWithRanges(tag.Keyword);
                        if (result?.Count > 0)
                        {
                            return result;
                        }
                    }
                }
                else
                {
                    return value.Ranges;
                }
            }
            return Array.Empty<Range>().ToList();

        }

        internal string GetKeywordForRange(Range selectedRange)
        {
            foreach (var kvp in from kvp in Index
                                from item in kvp.Value.Ranges
                                where item?.Contains(selectedRange) ?? false
                                select kvp)
            {
                return kvp.Key;
            }

            return null;
        }

        public void SelectKeyword(string keyword, Range range = null) => ScrollToRangeInBrowser(range ?? Index[keyword].Ranges.First());

        internal Range[] GetRangesForKeyword(string keyword) => Index[keyword].Ranges.ToArray();
        internal void ShowAddTagDialog(out ComboBox parent, out Tuple<string, bool[]> result, string[] options = null, string defaultResult = "")
        {
            Label parentLabel = new Label
            {
                Text = "Parent"
            };
            CheckBox isNameCheckBox = new CheckBox
            {
                Text = "Name",
            };
            isNameCheckBox.CheckedChanged += IsNameCheckBox_CheckedChanged;
            string[] items = GetKeys().ToArray();
            parent = InputDialog<string>.CreateComboBox("", items);
            Control[] flowControls = new Control[] { isNameCheckBox, parentLabel, parent };
            
            result = InputDialog<string>.ShowDialog(
                caption: "Add Tag",
                labelText: "Tag Name:",
                defaultResult: defaultResult,
                items: items,
                options: options,
                flowControls: flowControls
                );
        }

        private void IsNameCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var checkbox = (CheckBox)sender;
            var form = checkbox.FindForm();
            if (form == null) return;
            var combo = form.Controls[1] as ComboBox;
            if (combo == null) return;
            combo.Text = combo.Text.SwapLastNameFirst();            

        }

        public void CleanupHighlights()
        {
            int position = _richTextBox.SelectionStart;
            RemoveHighlight(new Range(0, Range.GetEndFromLength(0, _richTextBox.TextLength)));
            foreach (var kvp in Index)
            {             
                kvp.Value.Ranges.ForEach(r => HighlightRange(r, kvp.Value.IsTag));
            }
            _richTextBox.SelectionStart = position;
            MessagePanel.Show(_richTextBox, "Highlights have been refreshed.", durationInSeconds: 3);
        }
        private Dictionary<string, T> ToCaseInsensitiveDictionary<T>(Dictionary<string, T> originalDictionary, StringComparer comparer = null)
            => new Dictionary<string, T>(originalDictionary, comparer ?? StringComparer.OrdinalIgnoreCase);

        Range ICheckBoxSelectionOwner.GetRangeById(string indexSourceId)
        {
            foreach (var range in from kvp in Index
                                  from range in kvp.Value.Ranges
                                  where range.Id == indexSourceId
                                  select range)
            {
                return range;
            }

            return null;
        }

        Dictionary<string, List<Range>> IRangeListContainer.GetIndex()
        {
            return GetIndex();
        }
    }
}
