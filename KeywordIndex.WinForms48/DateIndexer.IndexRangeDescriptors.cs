using Journadex.Library;
using System;

namespace KeywordIndex.WinForms48
{
    public partial class DateIndexer
    {
        internal class IndexRangeDescriptors
        {
            private DateIndexer Owner { get; }

            public IndexRangeDescriptors(DateIndexer owner)
            {
                Owner = owner;
            }
            internal Tuple<string, string> FirstSentance(Range arg)
            {
                var date = Owner.GetDateForRange(arg)?.ToLongDateString() ?? arg.ToRangeString(useLength: false);
                string firstSentance = Owner.RichTextBox.GetFirstSentanceAfterNewLine(arg);
                return Tuple.Create(date, firstSentance);
            }

            internal Tuple<string, string> DateWithSnippet(Range arg) 
                => GetDateWithSnippet(arg, Owner.GetDateForRange(arg));
            internal Tuple<string, string> EntryDateWithSnippet(Range arg)
                => GetDateWithSnippet(arg, Owner.GetDateForRange(arg));
            private Tuple<string, string> GetDateWithSnippet(Range arg, DateTime? dateTime)
            {
                var date = dateTime?.ToShortDateString() ?? arg.ToRangeString(useLength: false);
                var snippet = Owner.GetHtmlSnippet(arg) ?? string.Empty;
                return Tuple.Create(date, snippet);
            }
        }
    }
}
