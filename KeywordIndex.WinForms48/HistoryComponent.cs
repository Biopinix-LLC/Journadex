using Journadex.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    /// <summary>
    /// ChatGPT: Create a C# HistoryComponent class that takes a RichTextBox and provides methods for Add, Clear, Next and Previous. The history should be stored as Range objects. Range is an existing class with Start and End integer properties and a calculated length.
    /// </summary>
    public class HistoryComponent : IRangeListContainer 
    {
        internal const string IndexKey = "History*";
        private readonly RichTextBox _target;

        public event EventHandler<RangeListChangedEventArgs> IndexChanged;

        public HistoryComponent(RichTextBox target)
        {
            _target = target;
            History = new List<Range>();
            CurrentIndex = -1;

            // Add an initial range for the current text
            AddSelectedRange();
        }

        public List<Range> History { get; }
        public int CurrentIndex { get; set; }
            
        public string GetDebugInfo(string header) => $"{header}: {CurrentIndex} of {History.Count} (pos: {History[CurrentIndex].Start}, len: {History[CurrentIndex].Length}";

        public void AddSelectedRange()
        {
            Range range = _target.GetSelectedRange();
            History.Add(range);
            CurrentIndex = History.Count - 1;
            IndexChanged?.Invoke(this, new RangeListChangedEventArgs(System.ComponentModel.ListChangedType.ItemAdded, IndexKey, range));
        }

        public void Clear()
        {
            History.Clear();
            CurrentIndex = -1;
        }

      
        public void Next()
        {
            if (CurrentIndex < History.Count - 1)
            {
                CurrentIndex++;
                Range range = History[CurrentIndex];
                _target.Select(range.Start, range.Length);
                _target.ScrollToCaret();
            }
        }

        public void Previous()
        {
            if (CurrentIndex > 0)
            {
                CurrentIndex--;
                Range range = History[CurrentIndex];
                _target.Select(range.Start, range.Length);
                _target.ScrollToCaret();
            }
        }

        public Dictionary<string, List<Range>> GetIndex()
        {
            return new Dictionary<string, List<Range>>
            {
                [IndexKey] = History
            };
        }
    }

    

}
