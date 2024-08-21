using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Journadex.Library
{
    /// <summary>
    /// ChatGPT: I have text dictated from several volumes of journals. The page numbers are at the bottom of the page in the journals and were dictated in the text as "page x". Write a C# class that takes the journal text, stores the range of the pages within the text, and returns the journal text with the page numbers removed. The class should allow me to get the volume and page number when given the location within the text. It should also allow me to get the range of the page within the text when given the volume and page number. I already have a Range class that has Start and End integer properties and a calculated Length property. Please optimize for memory.
    /// </summary>
    public class PageIndex
    {
        public Dictionary<int, Volume> Volumes { get; } = new Dictionary<int, Volume>();
        public StringBuilder Errors { get; private set; }

        public PageIndex()
        {


        }

        public string Build(string text)
        {
            Volumes.Clear();
            

           
            
            // Iterate through the matches and store the range of each page (excluding the "page x")
            StringBuilder sb = ParsePages(text);
            if (Errors != null)
            {
                throw new Exception(Errors.ToString());
            }

            return sb.ToString();

        }
        /// <summary>
        /// ChatGPT: Write a static C# 'ParsePages' method that takes a string of text with page breaks defined by "page x" at the end of each page and returns a StringBuilder with the "page x" removed. The method should also take a dictionary of Range objects keyed by a string of page numbers. The dictionary should be populated with the Range of each page in the StringBuilder.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private StringBuilder ParsePages(string text)
        {                     
            int prevPageNum = 0;
            int volumeNumber = 1;
            Volumes.Add(volumeNumber, new Volume());
            Volumes[volumeNumber].PageRanges = new Dictionary<string, Range>();
            StringBuilder sb = new StringBuilder();

            int startIndex = 0;
            int endIndex;
            string pattern = "page \\d+";
            Regex pageRegex = new Regex(pattern, RegexOptions.IgnoreCase);
            while ((endIndex = text.IndexOf("page ", startIndex, StringComparison.CurrentCultureIgnoreCase)) != -1)
            {
                Match pageMatch = pageRegex.Match(text, endIndex);
                if (!pageMatch.Success)
                {
                    break;
                }
                string pageNum = pageMatch.Value.Split(' ')[1];
                if (int.TryParse(pageNum, out var num))
                {
                    TryAddVolume(startIndex, num, ref volumeNumber, ref prevPageNum, sb.Length);
                }
                int pageNumEnd = pageMatch.Index + pageMatch.Length;
                int pageStart = sb.Length;
                sb.Append(text, startIndex, endIndex - startIndex);
                Volumes[volumeNumber].PageRanges.AddWithUniqueKey(pageNum, new Range(pageStart, sb.Length));

                startIndex = pageNumEnd + 1;
            }
            int lastPageStart = sb.Length;
            sb.Append(text, startIndex, text.Length - startIndex);

            // Add the range for the last page of the last volume
            Volumes[volumeNumber].Range = new Range(lastPageStart, sb.Length);
            return sb; // The numbers in the index look right but the selections in the navigator don't
        }

        private void TryAddVolume(int start, int pageNum, ref int volumeNumber, ref int prevPageNum, int outputLength)
        {
            if (prevPageNum == 0)
            {
                prevPageNum = pageNum;
            }
            else
            {
                int diff = pageNum - prevPageNum;
                if (diff > 2)
                {
                    if (Errors == null)
                    {
                        Errors = new StringBuilder();
                    }

                    for (int i = prevPageNum + 1; i <= pageNum; i++)
                    {
                        AppendPageError(volumeNumber, i).AppendLine(" skipped.");
                    }
                }
                else if (pageNum < prevPageNum)
                {
                    Volumes[volumeNumber].Range = new Range(0, outputLength); // TODO if supporting more than two volumes the starting range will need to be the previous volumes end + 1.
                    volumeNumber++;
                    if (volumeNumber > 2) throw new NotSupportedException("Only two volumes are supported.");
                    Volumes.Add(volumeNumber, new Volume());
                    Volumes[volumeNumber].PageRanges = new Dictionary<string, Range>();
                }
                prevPageNum = pageNum;
            }
        }

        public int GetVolume(int position)
        {
            // Iterate through the volumes and find the volume
            // corresponding to the given position
            foreach (var volume in Volumes)
            {
                if (volume.Value.Range.Start <= position && position <= volume.Value.Range.End)
                {
                    return volume.Key;
                }
            }

            // If no volume is found, return -1
            return -1;
        }

        public string GetPageNumber(int position)
        {
            // Iterate through the volumes and find the page number
            // corresponding to the given position
            foreach (var volume in Volumes)
            {
                if (volume.Value.Range.Start <= position && position <= volume.Value.Range.End)
                {
                    foreach (var pageRange in volume.Value.PageRanges)
                    {
                        if (pageRange.Value.Start <= position && position <= pageRange.Value.End)
                        {
                            return pageRange.Key;
                        }
                    }
                }
            }

            // If no page range is found, return null
            return null;
        }

        public Range GetPageRange(int volume, string pageNum)
        {
            if (Volumes.ContainsKey(volume) && Volumes[volume].PageRanges.ContainsKey(pageNum))
            {
                return Volumes[volume].PageRanges[pageNum];
            }

            // If the volume or page number is not found, return null
            return null;
        }
       

        private StringBuilder AppendPageError(int volumeNumber, int i)
        {
            return Errors.Append("Page number ").Append(volumeNumber + 1).Append('.').Append(i);
        }

        public class Volume
        {
            public Dictionary<string, Range> PageRanges { get; set; }
            public Range Range { get; set; }
        }
    }
}

    
    


