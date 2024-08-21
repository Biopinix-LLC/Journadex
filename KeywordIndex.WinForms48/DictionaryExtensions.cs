using Journadex.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using static KeywordIndex.WinForms48.DateIndexer;
using static KeywordIndex.WinForms48.KeywordIndexer;

namespace KeywordIndex.WinForms48
{
    internal static class DictionaryExtensions
    {
        public static void RemoveNullRanges(this Dictionary<string, IndexItemInfo> dictionary)
        {
            foreach (var kvp in dictionary)
            {
                kvp.Value.Ranges.RemoveAll(x => x == null);
            }
        }
     

        /// <summary>
        /// ChatGPT: Write a C# extension method for a Dictionary<string, List<Range>> that gets the index in the list of the range that contains a specified integer
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetRangeIndexContaining(this Dictionary<string, List<Range>> dict, int value)
        {
            foreach (var kvp in dict)
            {
                var rangeList = dict[kvp.Key];
                for (int i = 0; i < rangeList.Count; i++)
                {
                    if (rangeList[i].Start <= value && value <= rangeList[i].End)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        ///// <summary>
        ///// ChatGPT: Write a C# extension method for a Dictionary<DateTime, List<Range>> that returns all the dates that  match the Start and End value of the specified Range.
        ///// </summary>
        ///// <param name="dictionary"></param>
        ///// <param name="range"></param>
        ///// <returns></returns>
        //public static List<DateTime> GetDatesForRanges(this Dictionary<DateTime, List<Range>> dictionary, Range[] ranges)
        //{
        //    return dictionary
        //        .Where(x => x.Value.Any(r => ranges.Any(d => d.Start == r.Start && d.End == r.End)))
        //        .Select(x => x.Key)
        //        .ToList();
        //}

        public static List<DateTime> GetDatesForRanges(this Dictionary<DateTime, DateIndexItem> dictionary, Range[] ranges)
        {
            // TODO works 99% of the time. What about `dad` on Februrary 28, 2001?
            var dates = new List<DateTime>();

            foreach (var kvp in dictionary)
            {
                var entryRange = kvp.Value.Entry;
                List<Range> dateReferences = kvp.Value.DateReferences;
                bool noDateReferences = dateReferences == null || dateReferences.Count == 0;
                foreach (var specifiedRange in ranges)
                {
                    if (noDateReferences && entryRange != null && entryRange.Contains(specifiedRange))
                    {
                        // If the entry range exists and overlaps with any of the specified ranges, add the corresponding DateTime key
                        dates.Add(kvp.Key);
                        continue;
                    }

                    if (dateReferences == null) continue;
                    foreach (var range in dateReferences)
                    {

                        if (range.Contains(specifiedRange))
                        {
                            // If the specified range overlaps with any of the DateReferences ranges, add the corresponding DateTime key
                            dates.Add(kvp.Key);
                            break;
                        }

                    }
                }
            }

            return dates;
        }





    }
}
