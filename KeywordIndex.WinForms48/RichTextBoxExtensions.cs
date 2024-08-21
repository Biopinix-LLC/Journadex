using Journadex.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    internal static class RichTextBoxExtensions
    {
        internal static Range GetSelectedRange(this RichTextBox richTextBox) => new Range(richTextBox.SelectionStart, Range.GetEndFromLength(richTextBox.SelectionStart, richTextBox.SelectionLength));

        internal enum TrimType { Both, AtStart, AtEnd }
        /// <summary>
        /// Gets the specified range within the rich text box with the leading and trailing non-alphanumeric characters removed.
        /// </summary>
        /// <param name="richTextBox"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        internal static Range TrimRange(this RichTextBox richTextBox, Range range, TrimType trimType = TrimType.Both)
        {
            // Get the start and end indices of the range
            int startIndex = range.Start;
            int endIndex = range.End;

            if (trimType == TrimType.Both || trimType == TrimType.AtStart)
            {
                // Trim the leading non-alphanumeric characters
                while (startIndex < endIndex && !char.IsLetterOrDigit(richTextBox.Text[startIndex]))
                {
                    startIndex++;
                }
            }
            if (trimType == TrimType.Both || trimType == TrimType.AtEnd)
            {
                // Trim the trailing non-alphanumeric characters
                while (endIndex > startIndex && !char.IsLetterOrDigit(richTextBox.Text[endIndex]))
                {
                    endIndex--;
                }
            }
            // Create and return a new range with the trimmed indices
            return new Range(startIndex, endIndex);
            //return richTextBox.Find(richTextBox.Text.Substring(startIndex, endIndex - startIndex), startIndex, endIndex, RichTextBoxFinds.None);
        }

        internal static void SelectCurrentWord(this RichTextBox rtb, int index)
        {
            // Find the start and end indexes of the current word
            int startIndex = rtb.Text.LastIndexOf(" ", index) + 1;
            if (startIndex < 0) startIndex = 0; // handle beginning of text
            int endIndex = rtb.Text.IndexOf(" ", index);
            if (endIndex < 0) endIndex = rtb.Text.Length; // handle end of text

            var range = rtb.TrimRange(new Range(startIndex, endIndex), TrimType.AtEnd);

            // Select the current word
            rtb.Select(range.Start, range.Length);
        }

        internal static bool IsCursorInsideSelection(this RichTextBox rtb)
        {
            // Get the index of the character under the mouse cursor
            Point mousePos = Cursor.Position;
            mousePos = rtb.PointToClient(mousePos);
            int index = rtb.GetCharIndexFromPosition(mousePos);

            // Check if the index is within the selection range
            return index >= rtb.SelectionStart && index < rtb.SelectionStart + rtb.SelectionLength;
        }

        internal static void SetBackgroundColor(this RichTextBox richTextBox, Range range, Color color)
        {
            // Check if the start index is before the end index
            if (range.Start > range.End)
            {
                throw new ArgumentException("The start index must be before the end index.");
            }

            // Get the text within the specified range
            string text = richTextBox.Text.Substring(range);

            // Select the text within the range
            richTextBox.Select(range.Start, range.Length);

            // Set the background color of the selected text
            richTextBox.SelectionBackColor = color;
        }

        internal static string GetFirstSentanceAfterNewLine(this RichTextBox journalRtf, Range arg)
        {
            string firstSentance;
            int start = arg.Start + 2;
            if (start < journalRtf.TextLength)
            {
                start = journalRtf.Text.IndexOf("\n", start);
                int end = journalRtf.Text.IndexOfAny(new char[] { '.', '!', '?' }, start); 
                if (end == -1) end = journalRtf.TextLength - 1;
                int length = end - start + 1;
                firstSentance = journalRtf.Text.Substring(start, length);
            }
            else
            {
                firstSentance = journalRtf.Text;
            }

            return firstSentance;
        }

        /// <summary>
        /// ChatGPT: I need a method that returns the sentence for the specified Range within the text. Range is a class of Start, End, and Length integers. If the range contains more than one sentence, only the first sentence should be returned. Sentences can end with a period, question mark or exclamation point. The range could be in the middle of a sentence. I need the entire sentence returned.
        /// </summary>
        /// <param name="richTextBox"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static string GetSentence(this RichTextBox richTextBox, Range range, out Range sentanceRange)
        {
            if (range == null)
            {
                sentanceRange = null;
                return string.Empty;
            }
            // Find the first sentence-ending character (period, question mark, exclamation point) within the range
            int endIndex = -1;
            for (int i = range.Start; i < range.Start + range.Length; i++)
            {
                if (richTextBox.Text[i].IsSentenceEnding())
                {
                    endIndex = i;
                    break;
                }
            }

            if (endIndex == -1)
            {
                // No sentence-ending character was found within the range, so find the end of the sentence
                endIndex = range.Start + range.Length;
                while (endIndex < richTextBox.Text.Length && !richTextBox.Text[endIndex].IsSentenceEnding())
                {
                    endIndex++;
                }
            }

            // Find the beginning of the sentence by searching backwards from the start of the range
            int startIndex = range.Start;
            while (startIndex > 0 && !richTextBox.Text[startIndex - 1].IsSentenceEnding())
            {
                startIndex--;
            }

            // Return the sentence from the beginning to the first sentence-ending character
            sentanceRange = new Range(startIndex, endIndex);
            return sentanceRange.Substring(richTextBox.Text);
        }

        public enum SearchType { Text, RegEx }
        public static class CompiledRegularExpressions
        {
            //private static readonly Pattern _allLinesStartingWithLongDatesWithOrdinalIndicators = 
            //    Pattern.With
            //           .StartOfLine
            //           .Word.
            public static readonly DateRegex FindAllLinesStartingWithLongDatesWithOrdinalIndicators =
                new DateRegex("^ *(Jan(uary)?|Feb(ruary)?|Mar(ch)?|Apr(il)?|May|Jun(e)?|Jul(y)?|Aug(ust)?|Sep(tember)?|Oct(ober)?|Nov(ember)?|Dec(ember)?) *([0-9]{1,2})(st|nd|rd|th)? *([0-9]{4})",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

            public static readonly RelativeDateRegex FindAllRelativeDates = new RelativeDateRegex(RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }
        public static Range[] FindAll(this RichTextBox richTextBox, string searchText = null, Range searchRange = null, SearchType type = SearchType.Text, Regex regex = null, DateTime? dateTime = null)
        {
            List<Range> matches = new List<Range>();
            int start = searchRange?.Start ?? 0;
            int length = searchRange?.Length ?? richTextBox.TextLength;
            if (regex != null && type != SearchType.RegEx)
            {
                type = SearchType.RegEx;
            }
            switch (type)
            {
                case SearchType.Text: // TODO still need to use this for adding selected text to index to see if it improves performance even more
                    {
                        if (string.IsNullOrEmpty(searchText)) throw new ArgumentNullException(nameof(searchText));
                        int index = richTextBox.Find(searchText, start, length, RichTextBoxFinds.None);
                        while (index != -1)
                        {
                            int end = index + searchText.Length;
                            matches.Add(new Range(index, end));
                            start = end;
                            length = searchRange.End - end;
                            index = richTextBox.Find(searchText, start, length, RichTextBoxFinds.None);
                        }
                    }

                    break;
                case SearchType.RegEx:
                    {
                        if (regex == null && string.IsNullOrEmpty(searchText)) throw new ArgumentNullException(nameof(searchText));
                        regex = regex ?? new Regex(searchText, RegexOptions.IgnoreCase);
                        var m = regex.Match(richTextBox.Text, start, length);
                        DateRegex dateRegex = null;
                        RelativeDateRegex relativeDateRegex = null;
                        if (regex is RelativeDateRegex rdr)
                        {
                            if (dateTime == null) throw new ArgumentNullException(nameof(dateTime), $"Argument is required when using {nameof(RelativeDateRegex)}.");
                            relativeDateRegex = rdr;
                        }
                        else if (regex is DateRegex dr)
                            dateRegex = dr;
                        while (m.Success)
                        {
                            int parentStart = m.Index;
                            int parentEnd = parentStart + m.Length - 1;
                            Range range = new Range(parentStart, parentEnd);
                            matches.Add(range);
                            dateRegex?.InvokeAll(m, range);
                            relativeDateRegex?.InvokeAll(m, range, dateTime.Value);
                            m = m.NextMatch();
                        }
                    }
                    break;
                default:
                    break;
            }
            return matches.ToArray();
        }

        ///// <summary>
        ///// ChatGPT: write a static method that attaches the FindPanel to a specified RichTextBox
        ///// </summary>
        ///// <param name="target"></param>
        //public static void AttachFindPanel(this RichTextBox target)
        //{
        //    if (target == null) throw new InvalidOperationException();
        //    target.HideSelection = false;
        //    FindPanel findPanel = new FindPanel(target);
        //    target.Parent.Controls.Add(findPanel);           
        //}


    }
}
