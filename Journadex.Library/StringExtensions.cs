using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Journadex.Library
{
    public static class StringExtensions
    {
        public static Range[] SplitWithPosition(this string str, params string[] separator)
        {
            // Split the string using the specified separator.
            var splitStr = str.Split(separator, System.StringSplitOptions.None);

            // Keep track of the start and end positions of each element.
            var ranges = new List<Range>();
            var currentPosition = 0;

            foreach (var element in splitStr)
            {
                // Store the start and end positions of the current element.
                int startPos = currentPosition;
                currentPosition += element.Length;
                int endPos = currentPosition - 1;

            }

            // Return the start positions and end positions.
            return ranges.ToArray();
        }
        /// <summary>
        /// ChatGPT: Write a safe substring method for C# that will clip the length to the end of the string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SafeSubstring(this string str, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            if (startIndex < 0)
            {
                startIndex = 0;
            }

            if (startIndex > str.Length - 1)
            {
                return string.Empty;
            }

            if (length < 0)
            {
                length = 0;
            }

            if (startIndex + length > str.Length)
            {
                length = str.Length - startIndex;
            }

            return str.Substring(startIndex, length);
        }

        public static string Substring(this Range range, string str) { return str.Substring(range); }
        public static string Substring(this string str, Range range) { return str.SafeSubstring(range.Start, range.Length); }

        /// <summary>
        /// Title case using Chicago style guidelines as interpreted by ChatGPT.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string str)
        {
            string[] words = str.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (word.Length > 0)
                {
                    // Always capitalize the first word
                    if (i == 0)
                    {
                        word = word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
                    }
                    // Capitalize all words except for articles, coordinating conjunctions, and prepositions
                    else if (!IsLowercaseWord(word))
                    {
                        word = word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
                    }
                    // Leave all other words lowercase
                    else
                    {
                        word = word.ToLower();
                    }
                    words[i] = word;
                }
            }
            return string.Join(" ", words);
        }

        private static bool IsLowercaseWord(string word)
        {
            // List of articles, coordinating conjunctions, and prepositions
            string[] lowercaseWords = { "a", "an", "and", "as", "at", "but", "by", "for", "if", "in", "nor", "of", "on", "or", "so", "the", "to", "up", "yet" };
            return lowercaseWords.Contains(word.ToLower());
        }

        private static readonly Dictionary<string, Func<DateTime, DateTime>> DateChanges = new Dictionary<string, Func<DateTime, DateTime>>()
{
    { "next month", d => d.AddMonths(1) },
    { "last month", d => d.AddMonths(-1) },
    { "next week", d => d.AddDays(7) },
    { "last week", d => d.AddDays(-7) },
    { "tomorrow", d => d.AddDays(1) },
    { "yesterday", d => d.AddDays(-1) },
    //{ "today", d => d },
    { "next monday", d => GetNextWeekday(d, DayOfWeek.Monday) },
    { "last monday", d => GetNextWeekday(d, DayOfWeek.Monday, -1) },
    { "next tuesday", d => GetNextWeekday(d, DayOfWeek.Tuesday) },
    { "last tuesday", d => GetNextWeekday(d, DayOfWeek.Tuesday, -1) },
    { "next wednesday", d => GetNextWeekday(d, DayOfWeek.Wednesday) },
    { "last wednesday", d => GetNextWeekday(d, DayOfWeek.Wednesday, -1) },
    { "next thursday", d => GetNextWeekday(d, DayOfWeek.Thursday) },
    { "last thursday", d => GetNextWeekday(d, DayOfWeek.Thursday, -1) },
    { "next friday", d => GetNextWeekday(d, DayOfWeek.Friday) },
    { "last friday", d => GetNextWeekday(d, DayOfWeek.Friday, -1) },
    { "next saturday", d => GetNextWeekday(d, DayOfWeek.Saturday) },
    { "last saturday", d => GetNextWeekday(d, DayOfWeek.Saturday, -1) },
    { "next sunday", d => GetNextWeekday(d, DayOfWeek.Sunday) },
    { "last sunday", d => GetNextWeekday(d, DayOfWeek.Sunday, -1) },
};

        /// <summary>
        /// ChatGPT: For a C# class that has a string stored in a Text property and using a Range class with Start and End integer properties, write a method that takes a specified Range inside that text and locates the Range of any words (not case-sensitive) that refer to a date such as "next Tuesday", "last month", "yesterday", "tomorrow", "today", "in two weeks", etc. and calculates the date relative to a given date. For example, given a date of 12/14/2022, "next month" would return a date of 01/01/2023 with the Range where "next month" is found in Text. Other examples for a given date of 12/14/2022:
        ///- "next Tuesday" returns 12/20/2022
        ///- "last month" returns 11/01/2022
        /// - "in two weeks" returns 12/28/2022
        /// can you update it to be a Try method that returns a bool when a match is found and outs all the matches as a DateTime and also include the absolute Range where each match was found in the text?
        /// the output range should go to the end of the sentence that the date is found in
        /// </summary>
        /// <param name="range"></param>
        /// <param name="baseDate"></param>
        /// <param name="dates"></param>
        /// <param name="ranges"></param>
        /// <returns></returns>
        public static bool TryGetInnerDatesFromText(this string Text, Range range, DateTime baseDate, out List<DateTime> dates, out List<Range> ranges)
        {
            // Initialize the lists of dates and ranges to empty lists.
            dates = new List<DateTime>();
            ranges = new List<Range>();

            // Get the text within the specified range.
            string text = Text.Substring(range.Start, range.End - range.Start);

            // Make the text lowercase so that we can match it against the keywords
            // without worrying about case sensitivity.
            text = text.ToLower();


            // Check if the text matches any of the keywords in the dictionary.
            foreach (var kvp in DateChanges)
            {
                if (text.Contains(kvp.Key))
                {
                    // If a match is found, apply the relative date change to get the resulting date.
                    DateTime date = kvp.Value(baseDate);

                    //// Find the end of the sentence that contains the date reference.
                    //int endOfSentence = text.IndexOf(kvp.Key) + kvp.Key.Length;
                    //while (endOfSentence < text.Length && text[endOfSentence] != '.')
                    //{
                    //    endOfSentence++;
                    //}

                    // Add the resulting date and the corresponding range to the output lists.
                    dates.Add(date);
                    //ranges.Add(new Range() { Start = range.Start + text.IndexOf(kvp.Key), End = range.Start + endOfSentence });
                    ranges.Add(new Range() { Start = range.Start + text.IndexOf(kvp.Key), End = range.Start + text.IndexOf(kvp.Key) + kvp.Key.Length });
                }
            }

            // Return true if at least one date reference was found, or false if not.
            return dates.Count > 0;
        }



        // ChatGPT: Helper method that returns the next or previous occurrence of a given day of the week.
        private static DateTime GetNextWeekday(DateTime date, DayOfWeek dayOfWeek, int direction = 1)
        {
            int daysToAdd = ((int)dayOfWeek - (int)date.DayOfWeek + 7 * direction) % 7;
            return date.AddDays(daysToAdd);
        }

        public static bool TryParseDateFromLine(this string line, out DateTime date, out string formattedDate)
        {
            // Parse the date from the line. You may want to customize this
            // to handle different date formats.
            var entry = line.Trim().Replace("rd", "").Replace("th", "").Replace("st", "").Replace("nd", "");
            var words = entry.Split(' ');
            // Check if the entry is a date line.
            if (words.Length >= 3)
            {
                string first3Words = string.Join(" ", words.Take(3));
                if (DateTime.TryParse(first3Words, out date))
                {
                    formattedDate = first3Words;
                    return true;
                }
            }

            date = default;
            formattedDate = null;
            return false;
        }

        public static bool HasDateLines(this string text)
        {
            var lines = text?.Split(new[] { '\n' }) ?? Array.Empty<string>();
            if (lines.Length == 0)
            {
                return false;
            }
            foreach (var line in lines)
            {
                if (line.TryParseDateFromLine(out _, out _))
                {
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// ChatGPT: Write a C# string extension method that takes the last word of a name string and puts the last name first separated by a coma. If it is already in this format it reverts it back to the last name at the end.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string SwapLastNameFirst(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            var words = name.Trim().Split(' ');

            if (words.Length == 1)
            {
                return name;
            }

            var lastNameIndex = words.Length - 1;

            if (words[0].EndsWith(","))
            {
                // Already in the format "Last, First", swap it back to "First Last"
                var firstName = string.Join(" ", words, 1, lastNameIndex).Trim();
                var lastName = words[0].TrimEnd(',');

                return $"{firstName} {lastName}";
            }
            else
            {
                // Not in the format "Last, First", swap it to "Last, First"
                var firstName = words[0];
                var lastName = string.Join(" ", words, 1, lastNameIndex);

                return $"{lastName}, {firstName}";
            }
        }

        public static int CalculateReadingTimeInSeconds(this string message)
        {
            int wordCount = 0;
            int characterCount = 0;

            // Count the number of words and characters in the message
            foreach (string word in message.Split(' '))
            {
                wordCount++;
                characterCount += word.Length;
            }

            // Calculate the estimated reading time in seconds
            double estimatedReadingTimeInMinutes = (double)characterCount / 5 / 200;
            int estimatedReadingTimeInSeconds = (int)Math.Ceiling(estimatedReadingTimeInMinutes * 60);

            return estimatedReadingTimeInSeconds;
        }

        public static string Truncate(this string input, int maxLength, ITruncateStrategy strategy, bool useEllipsis = true) 
            => strategy.Truncate(input, maxLength, useEllipsis);

        /// <summary>
        /// Replaces new lines with a single space
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveNewLines(this string input)
            => !string.IsNullOrEmpty(input) ? input.Replace("\r\n", "\n")
                    .Replace("\n", " ")
                    .Replace("  ", " ")
                    .Trim()
            : input;
    }

}