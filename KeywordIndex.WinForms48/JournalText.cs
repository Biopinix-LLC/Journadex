using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace KeywordIndex.WinForms48
{
    /// <summary>
    /// ChatGPT: I have text that was taken from several volumes of journals. The page number from the journal appears throughout the text in the format "page x" or "Page x". I want to populate a WinForms RichTextBox with the text excluding the page numbers. I also want to be able to navigate to a specific page in a specific volume of the journal which will select that text in the RichTextBox. I also want to be able to know what volume and page my caret is in. Please implement a C# class that does this.
    /// > please update RemovePageNumbers to use Regex to find the page number. The page number could be in the middle of a sentence not necessarily at the end of a line.
    /// </summary>
    public class JournalText
    {
        private RichTextBox richTextBox;
        private List<Tuple<int, int>> pageLocations;
        private List<int> volumes;
        private string _originalText;

        public string OriginalText
        {
            get => _originalText;
            set
            {
                _originalText = value;
            }
        }

        private CursorCoordinator CursorCoordinator { get; set; }
        internal JournalText(RichTextBox richTextBox, CursorCoordinator cursorCoordinator)
        {
            this.richTextBox = richTextBox;
            CursorCoordinator = cursorCoordinator;
            pageLocations = new List<Tuple<int, int>>();
            volumes = new List<int> { 0 };
            OriginalText = richTextBox.Text;
            RemovePageNumbers();
        }

        private void RemovePageNumbers()
        {
            string text = richTextBox.Text;
            Regex pageNumberRegex = new Regex(@"(page \d+)", RegexOptions.IgnoreCase);
            Match match = pageNumberRegex.Match(text);
            int lastPageNum = 0;
            int lastPageIndex = 0;
            while (match.Success)
            {
                int pageIndex = match.Index;
                int pageNumber = int.Parse(match.Groups[1].Value.Split(' ')[1]);
                if (pageNumber < lastPageNum)
                {
                    volumes.Add(lastPageIndex + 1);
                }
                lastPageNum = pageNumber;
                lastPageIndex = pageIndex;
                pageLocations.Add(new Tuple<int, int>(pageIndex, pageNumber));
                text = text.Remove(pageIndex, match.Length);
                match = pageNumberRegex.Match(text, pageIndex);
            }
            richTextBox.Text = text;
        }


        public void NavigateToPage(int volume, int page)
        {
            volume--;
            int startIndex = 0;
            int endIndex = 0;
            int volumeStartIndex, volumeEndIndex;
            GetVolumeRange(volume, out volumeStartIndex, out volumeEndIndex);
            // TODO Almost! Edge cases are still not working . Check GetCurrentVolumeAndPage
            for (int i = 0; i < pageLocations.Count; i++)
            {
                Tuple<int, int> pageLocation = pageLocations[i];
                if (pageLocation.Item2 == page && pageLocation.Item1 >= volumeStartIndex && pageLocation.Item1 <= volumeEndIndex)
                {
                    if (i > 0)
                    {
                        startIndex = pageLocations[i - 1].Item1 + 1;
                    }
                    endIndex = pageLocation.Item1;
                    break;
                }
            }

            CursorCoordinator.SetCursor(startIndex, endIndex - startIndex);
        }


        private void GetVolumeRange(int volume, out int volumeStartIndex, out int volumeEndIndex)
        {
            volumeStartIndex = volumes[volume];
            volumeEndIndex = volume + 1 < volumes.Count ? volumes[volume + 1] - 1 : richTextBox.TextLength - 1;
        }

        public Tuple<int, int> GetCurrentVolumeAndPage()
        {
            int caretIndex = richTextBox.SelectionStart;
            int volume = 0;
            for (int i = 0; i < volumes.Count; i++)
            {
                if (caretIndex >= volumes[i])
                {
                    volume = i;
                }
                else
                {
                    break;
                }
            }
            int volumeStartIndex, volumeEndIndex;
            GetVolumeRange(volume, out volumeStartIndex, out volumeEndIndex);
            int page = 1;
            foreach (var pageLocation in pageLocations)
            {
                if (pageLocation.Item1 <= caretIndex && pageLocation.Item1 >= volumeStartIndex && pageLocation.Item1 <= volumeEndIndex)
                {
                    page = pageLocation.Item2;
                }
                else if (pageLocation.Item1 > caretIndex)
                {
                    break;
                }
            }
            return new Tuple<int, int>(volume + 1, page);
        }

    }
}


