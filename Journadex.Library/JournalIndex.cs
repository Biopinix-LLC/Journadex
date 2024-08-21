using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journadex.Library
{
    //internal class JournalIndex
    //{
    //    public JournalIndex(string journalText)
    //    {
    //        Text = journalText;
    //        EntriesByDate = ParseJournalEntries(journalText);
    //    }

    //    public string Text { get; }
    //    public Dictionary<DateTime, Range> EntriesByDate { get; }

    //    private Dictionary<DateTime, Range> ParseJournalEntries(string journalText)
    //    {
    //        var entries = journalText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
    //        var result = new Dictionary<DateTime, Range>();
    //        var lastIndex = 0;
    //        var currentIndex = 0;

    //        for (int i = 0; i < entries.Length; i++)
    //        {
    //            var entry = entries[i].Trim();

    //            // Check if the entry is a date line.
    //            var isDateLine = DateTime.TryParse(entry, out var date);
    //            if (isDateLine)
    //            {
    //                // If the entry is a date line, add a new range to the result dictionary.
    //                result.Add(date, new Range { Start = lastIndex, End = currentIndex });

    //                // Update the last index.
    //                lastIndex = currentIndex;
    //            }

    //            // Increment the current index by the length of the entry plus one for the newline characters.
    //            currentIndex += entry.Length + 2;
    //        }

    //        return result;
    //    }

    //}
}
