
    using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Journadex.Library
{
    /// <summary>
    /// ChatGPT: Write a C# class that inherits from RegEx that matches phrases such as "yesterday", "tomorrow", "next week/month/year/Sunday/Monday/Tuesday/Wednesday/Thursday/Friday/Saturday", "last week/month/year/Sunday/Monday/Tuesday/Wednesday/Thursday/Friday/Saturday", "x days/weeks/months ago" and exposes an Action<Match, Range, DateTime> property. There should also be a method that calculates a date from the match relative to a specified date.
    /// </summary>
    public class RelativeDateRegex : Regex
    {      
        public RelativeDateRegex(RegexOptions options = RegexOptions.IgnoreCase) 
            : base(@"\b(yesterday|tomorrow|next (week|month|year|Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday)|last (week|month|year|Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday)|([0-9]+)\s(days?|weeks?|months?)\sago)\b", options) { }

        public static DateTime CalculateDate(Match match, DateTime relativeTo)
        {
            // TODO performance could be improved
            if (match.Success)
            {
                string value = match.Value.ToLower();
                if (value == "yesterday")
                {
                    return relativeTo.AddDays(-1);
                }
                else if (value == "tomorrow")
                {
                    return relativeTo.AddDays(1);
                }
                else if (value.StartsWith("next"))
                {
                    string[] parts = value.Split(' ');
                    if (parts[1] == "week")
                    {
                        return relativeTo.AddDays(7);
                    }
                    else if (parts[1] == "month")
                    {
                        return relativeTo.AddMonths(1);
                    }
                    else if (parts[1] == "year")
                    {
                        return relativeTo.AddYears(1);
                    }
                    else
                    {
                        // Get the next occurrence of the specified day
                        DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), parts[1], ignoreCase: true);
                        int daysUntil = ((int)day - (int)relativeTo.DayOfWeek + 7) % 7;
                        if (daysUntil == 0) daysUntil += 7;
                        return relativeTo.AddDays(daysUntil);
                    }
                }
                else if (value.StartsWith("last"))
                {
                    string[] parts = value.Split(' ');
                    if (parts[1] == "week")
                    {
                        return relativeTo.AddDays(-7);
                    }
                    else if (parts[1] == "month")
                    {
                        return relativeTo.AddMonths(-1);
                    }
                    else if (parts[1] == "year")
                    {
                        return relativeTo.AddYears(-1);
                    }
                    else
                    {
                        // Get the last occurrence of the specified day
                        DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), parts[1], ignoreCase: true);
                        int daysUntil = ((int)day - (int)relativeTo.DayOfWeek - 7) % 7;
                        if (daysUntil == 0) daysUntil -= 7;
                        return relativeTo.AddDays(daysUntil);
                    }
                }
                else
                {
                    // x days/weeks/months ago
                    string[] parts = value.Split(' ');
                    int amount = int.Parse(parts[0]);
                    if (parts[1] == "day" || parts[1] == "days")
                    {
                        return relativeTo.AddDays(-amount);
                    }
                    else if (parts[1] == "week" || parts[1] == "weeks")
                    {
                        return relativeTo.AddDays(-amount * 7);
                    }
                    else if (parts[1] == "month" || parts[1] == "months")
                    {
                        return relativeTo.AddMonths(-amount);
                    }
                }
            }
            return relativeTo;
        }

        public Dictionary<string, Action<Match, Range, DateTime>> Actions { get; } = new Dictionary<string, Action<Match, Range, DateTime>>();
        public void InvokeAll(Match match, Range range, DateTime dateTime)
        {
            foreach (var kvp in Actions)
            {
                kvp.Value.Invoke(match, range, dateTime);
            }
        }


    }
}
