using System;
using System.Collections.Generic;
using Journadex.Library;
namespace KeywordIndex.WinForms48
{
    public partial class KeywordIndexer
    {
        public class IndexItemInfo
        {
            public IndexItemInfo(string keyword, Range range, bool isTag = false)
            {
                if (range != null)
                {
                    Add(range);
                }
                Keyword = keyword;
                IsTag = isTag;
            }

            public List<Range> Ranges { get; } = new List<Range>();
            public string Keyword { get; }
            public bool IsTag { get; }
            public List<IndexItemInfo> Children { get; } = new List<IndexItemInfo>();
            
            public IndexItemInfo Parent { get; set; }
            public List<string> Metadata { get; } = new List<string>();
            internal void Add(Range range) => Ranges.Add(range);
            internal void SetParent(IndexItemInfo parent)
            {
                // I refactored this originally from the Parent set to try to keep this from being called during deserialization.
                // Outline Node appears to work similarly. However, when I made this Parent's set private, it did not load properly.
                if (Parent != null)
                {
                    Parent.Children.Remove(this);
                }

                Parent = parent;

                if (Parent != null && !Parent.Children.Contains(this))
                {
                    Parent.Children.Add(this);
                }
            }
        }
    }
}
