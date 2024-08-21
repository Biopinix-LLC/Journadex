using Journadex.Library;
using System.ComponentModel;

namespace KeywordIndex.WinForms48
{
    public class RangeListChangedEventArgs
    {
        public ListChangedType ItemAdded { get; }

        public string IndexKey { get; }
        public Range Range { get; }

        public RangeListChangedEventArgs(ListChangedType itemAdded, string indexKey, Range range)
        {
            ItemAdded = itemAdded;
            IndexKey = indexKey;
            Range = range;
        }
    }
}