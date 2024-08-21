using Journadex.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
namespace KeywordIndex.WinForms48
{
    public partial class KeywordIndexer
    {
        

        private class KeywordIndexContextMenuHandler : WebBrowserContextMenuHandler
        {
            private const string ValidHrefPrefix = "about:blank#";
            private static readonly int ValidHrefPrefixLength = ValidHrefPrefix.Length;
            private readonly KeywordIndexer _indexer;
            private IndexItemInfo _item;
            private string _alias;
            private Range _range;

            private IdType CurrentIdType { get; set; }
            private string CurrentKeyword { get; set; }

            public KeywordIndexContextMenuHandler(WebBrowser webBrowser, KeywordIndexer indexer) : base(webBrowser)
            {
                if (indexer == null) throw new ArgumentNullException(nameof(indexer));
                _indexer = indexer;
            }

            protected override bool PopulateMenu(ContextMenuStrip contextMenu)
            {
                if (!_indexer.Index.TryGetValue(CurrentKeyword, out _item)) return false;

                _alias = null;
                _range = null;

                switch (CurrentIdType)
                {
                    case IdType.Null:
                    case IdType.Page:
                        return false;
                    case IdType.Item:
                        break;
                    case IdType.Alias:
                        _alias = CurrentId;
                        break;
                    case IdType.Tag:
                        break;
                    case IdType.Range:
                        _range = Range.Parse(CurrentId);
                        break;
                    default:
                        break;
                }
                if (_item != null && CurrentKeyword != null)
                {

                    contextMenu.Items.Add($"Remove Item '{CurrentKeyword}'", null, OnRemoveItem);

                }
                if (_alias != null && CurrentKeyword != null)
                {
                    contextMenu.Items.Add($"Remove Alias '{_alias}' from '{CurrentKeyword}'", null, OnRemoveAlias);
                }

                if (_item.Parent == null)
                {
                    contextMenu.Items.Add($"Add Parent Tag to '{_item.Keyword}'", null, OnAddParentTag);
                }
                else
                {
                    contextMenu.Items.Add($"Change Parent '{_item.Parent.Keyword}' of '{_item.Keyword}'", null, OnChangeParentTag);
                    contextMenu.Items.Add($"Remove Parent '{_item.Parent.Keyword}' of '{_item.Keyword}'", null, OnRemoveParentTag);

                }
                contextMenu.Items.Add($"Add Child Tag to '{_item.Keyword}'", null, OnAddChildTag);
                if (_item != null && CurrentKeyword != null && _item.Children.Any(t => t.Keyword.Equals(_item.Keyword, StringComparison.InvariantCultureIgnoreCase)))
                {
                    contextMenu.Items.Add($"Remove Tag '{_item.Keyword}' from {CurrentKeyword}", null, OnRemoveTagFromItem);
                    // Don't offer to remove all tags from here - make them delete from the item
                }



                if (_range != null)
                {
                    var refDesc = _indexer.GetIndexRangeDescription(_range).Item1;
                    contextMenu.Items.Add($"Move '{refDesc}' Reference To...", null, OnMoveRangeTo);
                    contextMenu.Items.Add($"Remove '{refDesc}' Reference", null, OnRemoveRange);

                }
                return true;
            }

            private void OnRemoveRange(object sender, EventArgs e)
            {
                var refDesc = _indexer.GetIndexRangeDescription(_range);
                if (MessageBox.Show($"Remove '{refDesc.Item2}' from index? (This cannot be undone)", $"Remove Reference to {CurrentKeyword} on {refDesc.Item1}?", MessageBoxButtons.YesNo, icon: MessageBoxIcon.Warning, defaultButton: MessageBoxDefaultButton.Button2)
                    == DialogResult.Yes)
                {
                    _item.Ranges.RemoveAll(r => r.Start == _range.Start && r.End == _range.End);
                    _indexer.UpdateIndex();
                }
            }

            private void OnMoveRangeTo(object sender, EventArgs e)
            {
                var refDesc = _indexer.GetIndexRangeDescription(_range);
                try
                {
                    var (keyword, _) = InputDialog<string>.ShowDialog($"Move '{refDesc.Item1}' from '{CurrentKeyword}' To...", "Keyword", defaultResult: null, _indexer.GetKeys());
                
                if (string.IsNullOrEmpty(keyword)) return;
                if (!_indexer.Index.ContainsKey(keyword))
                {
                    _indexer.AddToIndex(keyword);
                }
                var newItem = _indexer.Index[keyword];
                newItem.Ranges.AddRange(_item.Ranges.Where(r => r.Start == _range.Start && r.End == _range.End));
                _item.Ranges.RemoveAll(r => r.Start == _range.Start && r.End == _range.End);
                _indexer.UpdateIndex();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            private void OnRemoveTagFromItem(object sender, EventArgs e)
            {
                if (MessageBox.Show($"Remove '{_item.Keyword}' from '{CurrentKeyword}'? (This cannot be undone)", "Remove Tag?", MessageBoxButtons.YesNo, icon: MessageBoxIcon.Warning, defaultButton: MessageBoxDefaultButton.Button2)
                   == DialogResult.Yes)
                {
                    _item.Children.RemoveAll(t => t.Keyword.Equals(_item.Keyword, StringComparison.InvariantCultureIgnoreCase));
                    _indexer.UpdateIndex();
                }
            }
            private void OnAddChildTag(object sender, EventArgs e)
            {
                var (childName, _) = InputDialog<string>.ShowDialog($"Add Child Tag to '{_item.Keyword}'", "Child Tag", defaultResult: null, _indexer.GetKeys());
                if (string.IsNullOrEmpty(childName)) return;
                IndexItemInfo childItem = _indexer.GetOrAddItem(childName);
                childItem.SetParent(_item);
                _indexer.UpdateIndex();
            }

            

            private void OnRemoveParentTag(object sender, EventArgs e)
            {
                if (MessageBox.Show($"Remove '{_item.Parent.Keyword}' from '{_item.Keyword}'? (This cannot be undone)", "Remove Parent Tag?", MessageBoxButtons.YesNo, icon: MessageBoxIcon.Warning, defaultButton: MessageBoxDefaultButton.Button2)
                    == DialogResult.Yes)
                {
                    _item.SetParent(null);
                    _indexer.UpdateIndex();
                }
            }

            private void OnChangeParentTag(object sender, EventArgs e) => SetTagParentWithDialog("Change", _item.Parent?.Keyword);

            private void OnAddParentTag(object sender, EventArgs e) => SetTagParentWithDialog("Add"); 

            private void SetTagParentWithDialog(string actionCaption, string defaultResult = null)
            {
                var (parentName, _) = InputDialog<string>.ShowDialog($"{actionCaption} Parent Tag", "Parent Tag", defaultResult: defaultResult, _indexer.GetKeys());
                if (string.IsNullOrEmpty(parentName)) return;
                _item.SetParent(_indexer.GetOrAddItem(parentName));
                _indexer.UpdateIndex();
            }

            private void OnRemoveAlias(object sender, EventArgs e)
            {
                if (MessageBox.Show($"Remove '{_alias}' from '{CurrentKeyword}'? (This cannot be undone)", "Remove Alias?", MessageBoxButtons.YesNo, icon: MessageBoxIcon.Warning, defaultButton: MessageBoxDefaultButton.Button2)
                    == DialogResult.Yes)
                {
                    if (_indexer.Aliases[_alias] != CurrentKeyword) throw new InvalidOperationException();
                    _indexer.RemoveAlias(_alias, CurrentKeyword);
                }
            }

            private void OnRemoveItem(object sender, EventArgs e)
            {
                if (MessageBox.Show($"Remove '{CurrentKeyword}' from index? (This cannot be undone)", "Remove Item?", MessageBoxButtons.YesNo, icon: MessageBoxIcon.Warning, defaultButton: MessageBoxDefaultButton.Button2) 
                    == DialogResult.Yes)
                {
                    _indexer.Remove(CurrentKeyword);
                }
            }

            private void OnAddTagToItem(object sender, EventArgs e)
            {
                _indexer.ShowAddTagDialog(out ComboBox parent, out Tuple<string, bool[]> result);
                if (result?.Item1 is string tagName && !string.IsNullOrEmpty(tagName))
                {
                    _item.SetParent(!string.IsNullOrEmpty(parent?.Text) ? _indexer.GetOrAddItem(parent.Text) : null);
                    _item.Children.Add(_indexer.GetOrAddItem(tagName));
                    _indexer.UpdateIndex();
                }
            }

            protected override string CurrentId
            {
                get => base.CurrentId;
                set
                {
                    base.CurrentId = value;
                    if (value == null)
                    {
                        CurrentIdType = IdType.Null;
                        CurrentKeyword = null;
                    }
                }
            }
            public enum IdType { Null, Item, Alias, Tag, Range, Page }
            protected override string GetIdFromElement(HtmlElement element)
            {
                CurrentId = null;
                if (element.TagName == "H2")
                {
                    CurrentIdType = IdType.Item;
                    CurrentKeyword = element.InnerText.Trim();
                }
                else if (element.TagName == "SPAN")
                {
                    CurrentIdType = IdType.Alias;
                    CurrentKeyword = element.GetPreviousElementByTagName("H2")?.InnerText.Trim();
                }
                else if (element.TagName == "A")
                {
                    CurrentKeyword = element.GetPreviousElementByTagName("H2")?.InnerText.Trim();
                    string href = element.GetAttribute("HREF");
                    if (href.StartsWith(ValidHrefPrefix))
                    {
                        char firstChar = href[ValidHrefPrefixLength];
                        if (firstChar == 't')
                        {
                            CurrentIdType = IdType.Tag;
                            return href.Substring(ValidHrefPrefixLength + TagLinkPrefix.Length);
                        }
                        else if (char.IsDigit(firstChar))
                        {
                            CurrentIdType = IdType.Range;
                            return href.Substring(ValidHrefPrefixLength);
                        }
                        else if (firstChar == 'p')
                        {
                            CurrentIdType = IdType.Page;
                            return href.Substring(ValidHrefPrefixLength + PageLinkPrefix.Length);
                        }
                    }
                }
                return CurrentIdType != IdType.Null ? element.InnerText.Trim() : null;
            }
        }

        private IndexItemInfo GetOrAddItem(string keyword)
        {
            if (!Index.TryGetValue(keyword, out var item))
            {
                item = AddToIndex(keyword, isTag: true);
            }

            return item;
        }
    }
}
