using Journadex.Library;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace KeywordIndex.WinForms48
{
    public class DateRegex : Regex
    {
        public Dictionary<string, Action<Match, Range>> Actions { get; } = new Dictionary<string, Action<Match, Range>>();
        public void InvokeAll(Match match, Range range)
        {
            foreach (var kvp in Actions)
            {
                kvp.Value.Invoke(match, range);
            }
        }
        public DateRegex(string pattern, RegexOptions options) : base(pattern, options)
        {
        }
    }
}