using Journadex.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    public partial class DateIndexer : Indexer, IMonthCalendarDecorator, IIndexFileComponent, ICheckBoxSelectionOwner, IRangeListContainer
    {
        internal const string EntriesKey = "Entries*";
        internal const string DatesKey = "Dates*";
        private Dictionary<DateTime, DateIndexItem> _index = new Dictionary<DateTime, DateIndexItem>();

        private DateTime _lastDateSelected;

        public event EventHandler<RangeListChangedEventArgs> IndexChanged;

        public MonthCalendar Calendar { get; }


        public DateIndexer(RichTextBox journalTextBox, MonthCalendar calendar, WebBrowser dateIndexBrowser) : base(journalTextBox, dateIndexBrowser)
        {
            Calendar = calendar;
            RangeDescriptors = new IndexRangeDescriptors(this);
            GetIndexRangeDescription = RangeDescriptors.FirstSentance;
        }

        public void PopulateCalendar()
        {
            if (_index.Count == 0)
            {

                ParseJournalEntries();
            }
            if (_index.Count == 0) return;
            ResetBoldedDaysToDefault();
            var datesByPosition = GetOrderedDates(byPosition: true).Select(kvp => kvp.Key);
            Calendar.TodayDate = Calendar.MinDate = _lastDateSelected = Calendar.SelectionStart = Calendar.SelectionEnd =
                datesByPosition.First();
            Calendar.MaxDate = datesByPosition.Last();

            UpdateIndex();

        }

        internal void ResetBoldedDaysToDefault()
        {
            // Clear any existing dates on the calendar
            Calendar.RemoveAllBoldedDates();
            // Add the dates to the calendar
            Calendar.BoldedDates = _index.Keys.ToArray();
        }




        /// <summary>
        /// ChatGPT: search the index for a range matching the specified range. If a matching entry is found, it will return the date for that entry. If no matching entry is found, it will return null.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        internal DateTime? GetDateForRange(Range r)
        {


            // Find the entry in the dictionary with a range that contains the given start and end positions
            foreach (var item in _index)
            {
                if (item.Value.Entry != null && item.Value.Entry.Start <= r.Start && item.Value.Entry.End >= r.End)
                {
                    return item.Key;
                }
                
            }
            // We need to look through alll the entries before we look through the date references or we will return the date reference when we expect the entry.
            // For example, February 7th, 2001 has a reference to Februrary 4th, 2001. The date for the range is the entry but in this case we get the reference.
            // The reference should only get returned when there is no entry.
            foreach (var item in _index)
            {
                if (item.Value.DateReferences == null) continue;
                foreach (var range in item.Value.DateReferences)
                {
                    if (range.Start <= r.Start && range.End >= r.End)
                    {
                        return item.Key;
                    }
                }

            }

            // Return null if no matching entry was found
            return null;
        }



        /// <summary>
        /// ChatGPT: Returns a string formatted as HTML of a snippet from the journal, with the specified range text in italics
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        internal string GetHtmlSnippet(Range range)
        {
            // Extract the snippet text
            string snippet = RichTextBox.Text.Substring(range);

            // Find the start and end positions of the context words
            int contextStart = range.Start;
            int contextEnd = range.End;
            const int contextLength = 5;
            for (int i = 0; i < contextLength; i++)
            {
                contextStart = RichTextBox.Text.LastIndexOf(" ", contextStart - 1);
                if (contextStart < 0)
                {
                    contextStart = 0;
                    break;
                }
            }
            for (int i = 0; i < contextLength; i++)
            {
                contextEnd = RichTextBox.Text.IndexOf(" ", contextEnd + 1);
                if (contextEnd < 0)
                {
                    contextEnd = RichTextBox.Text.Length;
                    break;
                }
            }

            // Extract the context text
            string context = RichTextBox.Text.Substring(new Range(contextStart, contextEnd));

            // Italicize the snippet within the context text
            context = context.Replace(snippet, $"<i>{snippet}</i>");

            // Format the snippet as HTML
            return context;
        }


        private void ParseJournalEntries()
        {
            // Parse the journal entries and build the dictionary
            DateRegex dateRegEx = RichTextBoxExtensions.CompiledRegularExpressions.FindAllLinesStartingWithLongDatesWithOrdinalIndicators;
            dateRegEx.Actions[nameof(ParseJournalEntries)] = ParseJournalEntryUsingLongDateFormat;
            var dateRanges = RichTextBox.FindAll(regex: dateRegEx);

            if (dateRanges.Length == 0) return;

            AdjustRangeToFitEntry(dateRanges);

            // Find dates within entries and add to index
            RelativeDateRegex relativeDateRegex = RichTextBoxExtensions.CompiledRegularExpressions.FindAllRelativeDates;
            relativeDateRegex.Actions[nameof(RelativeDateRegex)] = ParseDateReferencesWithinEntry;

            foreach (var kvp in _index.Where(kvp2 => kvp2.Value.Entry != null).ToArray())
            {
                RichTextBox.FindAll(regex: relativeDateRegex, dateTime: kvp.Key, searchRange: kvp.Value.Entry);
            }
        }

        private void ParseDateReferencesWithinEntry(Match match, Range range, DateTime dateTime)
        {
            var relativeDateTime = RelativeDateRegex.CalculateDate(match, dateTime);
            AddRangeToIndex(relativeDateTime, range, isEntry: false);
        }

        private void AdjustRangeToFitEntry(Range[] dateRanges)
        {
            dateRanges.First().Start = 0;
            for (int i = 1; i < dateRanges.Length; i++)
            {
                dateRanges[i - 1].End = dateRanges[i].Start - 1;
            }
            dateRanges.Last().End = RichTextBox.TextLength - 1;
        }

        private void ParseJournalEntryUsingLongDateFormat(Match m, Range range)
        {
            // This concatenates all the groups starting with digits and prepends the second group.
            // The groups with values look like this: "February 4th 2001", "February", "ruary", "4", "th", "2001".
            // "ruary" and "th" are optional. The month should always be the 2nd group when using this format. 
            var groups = (from Group grp in m.Groups
                          where !string.IsNullOrEmpty(grp.Value) && char.IsDigit(grp.Value[0])
                          select grp.Value);
            var join = string.Join(" ", groups.Prepend(m.Groups[1].Value).ToArray());
            DateTime date = DateTime.Parse(join);
            AddRangeToIndex(date, range, isEntry: true);
        }

        private void AddRangeToIndex(DateTime key, Range range, bool isEntry)
        {

            if (!_index.TryGetValue(key, out var item))
            {
                item = new DateIndexItem();
                _index[key] = item;
            }
            if (isEntry)
            {
                item.Entry = range;
            }
            else
            {
                if (item.DateReferences == null)
                {
                    item.DateReferences = new List<Range> { range };
                }
                else
                {
                    item.DateReferences.Add(range);
                }
            }

        }

        private IEnumerable<KeyValuePair<DateTime, DateIndexItem>> GetOrderedDates(int year = 0, int month = 0, bool byPosition = false)
        {
            var results = from kvp in _index
                          orderby byPosition ? (kvp.Value.Entry?.Start ?? kvp.Value.DateReferences.FirstOrDefault()?.Start ?? -1) 
                          : kvp.Key.Ticks
                          select new KeyValuePair<DateTime, DateIndexItem>(kvp.Key, kvp.Value);
            if (year == 0 || month == 0) return results;
            return results.Where(kvp => kvp.Key.Year == year && kvp.Key.Month == month);
        }

        protected override void BuildHtmlBody(StringBuilder html)
        {
            DateTime currentDate = Calendar.SelectionStart;
            html.Append($"<h2>{currentDate:MMMM yyyy}</h2><br>");
            html.Append("<ul>");
            foreach (var entry in GetOrderedDates(currentDate.Year, currentDate.Month))
            {                
                RenderRange(html, entry.Key, entry.Value);
            }
            html.Append("</ul>");
        }
        private void RenderRange(StringBuilder html, DateTime key, DateIndexItem item)
        {
            html.Append("<li>");
            if (item.Entry != null)
            {
                RenderCheckBoxForRange(html, item.Entry);
                RenderRange(html, item.Entry);
            }
            else
            {
                html.Append(key.ToLongDateString()).Append(" (No Entry)");
            }
            html.Append("</li>");
            if (item.DateReferences?.Count > 0)
            {
                html.Append("<ul>");
                foreach (var child in item.DateReferences)
                {
                    html.Append("<li>");
                    RenderCheckBoxForRange(html, child);    
                    RenderRange(html, child, RangeDescriptors.EntryDateWithSnippet);
                    html.Append("</li>");
                }
                html.Append("</ul>");
            }
        }

        internal IndexRangeDescriptors RangeDescriptors { get; }

        bool ICheckBoxSelectionOwner.IsDateIndex => true;

        public void LoadFromIndexes(IIndexData indexes)
        {
            var info = indexes.Journal;
            if (info == null) return;

            if (info.Index != null)
                _index = info.Index;
           // PopulateCalendar();
        }

        public void SaveToIndexes(IIndexData indexes) => indexes.Journal = new JournalInfo { Index = _index };

        internal override Dictionary<string, List<Range>> GetIndex()
        {
            List<Range> entries = new List<Range>();
            List<Range> index = new List<Range>();
            Dictionary<string, List<Range>> result = new Dictionary<string, List<Range>>
            {
                [EntriesKey] = entries,
                [DatesKey] = index
            };
            entries.AddRange(from kvp in _index
                             where kvp.Value.Entry != null
                             select kvp.Value.Entry);
            foreach (var kvp in _index)
            {
                if (kvp.Value.Entry != null)
                {
                    index.Add(kvp.Value.Entry);
                }
                if (kvp.Value.DateReferences == null) continue;
                index.AddRange(kvp.Value.DateReferences);
            }
            index = index.OrderBy(r => r.Start).ToList();
            return result;
        }

        internal Range GetRangeForDate(DateTime date)
        {
            return _index.TryGetValue(date, out var item) ? item.Entry : null;
        }

        internal DateTime[] GetDatesForRanges(Range[] ranges)
        {

            return _index.GetDatesForRanges(ranges).ToArray();
        }

        internal override void ScrollToRangeInBrowser(Range selectedRange)
        {
            if (_lastDateSelected.Month != Calendar.SelectionStart.Month || _lastDateSelected.Year != Calendar.SelectionStart.Year)
            {
                UpdateIndex();
            }
            _lastDateSelected = Calendar.SelectionStart;
            base.ScrollToRangeInBrowser(selectedRange);
        }

        Range ICheckBoxSelectionOwner.GetRangeById(string indexSourceId)
        {
            return GetIndex().Values.SelectMany(r => r).FirstOrDefault(r => r.Id == indexSourceId);
        }

        Dictionary<string, List<Range>> IRangeListContainer.GetIndex()
        {
            return GetIndex();
        }

        public class JournalInfo
        {
            public Dictionary<DateTime, DateIndexItem> Index { get; set; }
        }

        public class DateIndexItem
        {
            public Range Entry { get; set; }
            public List<Range> DateReferences { get; set; }
        }
    }
}
