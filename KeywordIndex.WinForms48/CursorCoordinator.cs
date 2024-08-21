using System;
using System.Windows.Forms;
using Journadex.Library;

namespace KeywordIndex.WinForms48
{
    /// <summary>
    /// I need a class that will keep the cursor position of the rich text box in sync with the navigator, calendar and web browsers. For instance, If the user changes the cursor position in the rich text box, that date in the calendar should be selected and the date index web browser should scroll to and focus on the first link that is within that range. If the user selects a different day in the calendar, the cursor in the rich text box should change to the start of the first range for that date. If the cursor is within the range of a keyword in the KeywordIndex, the keyword index web browser should scroll to and focus on that keyword. The navigator should also move to that position and select that keyword in it's combo box. Care should be taken to make sure that there are no recursive loops.
    /// ChatGPT: It sounds like you need a new class that acts as a "coordinator" between the various classes and controls involved in keeping the cursor position in sync. This class can be responsible for handling the events that are raised when the cursor position changes in the RichTextBox, a different day is selected in the calendar, and when a keyword is selected in the KeywordIndex. It can also handle the logic for updating the cursor position in the RichTextBox, selecting the appropriate day in the calendar, scrolling and focusing on the appropriate link in the web browsers, and updating the navigator. To avoid recursive loops, you can use flags or other mechanism to keep track of the current state of the coordination and only take action if necessary.
    /// </summary>
    public class CursorCoordinator
    {
        private KeywordIndexer _keywordIndexer;
        private DateIndexer _dateIndexer;
        private readonly PageNavigator _pageNavigator;

        //private Outline _outline;
        private Navigator _navigator;
        private RichTextBox _richTextBox;
        private bool _updatingCursor;
        private bool _on;
        
        /// <summary>
        /// Gets or sets whether the RichTextBox and DateIndexer events should be fired 
        /// </summary>
        public bool On
        {
            get { return _on; }
            internal set
            {
                bool changed = _on != value;
                _on = value;
                if (!changed) return;
                if (On)
                {
                    _richTextBox.MouseUp += RichTextBox_MouseUp;
                    _dateIndexer.Calendar.DateSelected += Calendar_DateSelected;
                    // TODO _navigator.KeywordSelected += Navigator_KeywordSelected;
                    // _keywordIndexer.KeywordSelected += KeywordIndexer_KeywordSelected;                
                }
                else
                {
                    _richTextBox.MouseUp -= RichTextBox_MouseUp;
                    _dateIndexer.Calendar.DateSelected -= Calendar_DateSelected;
                    // TODO _navigator.KeywordSelected -= Navigator_KeywordSelected;
                    // _keywordIndexer.KeywordSelected -= KeywordIndexer_KeywordSelected;
                }
                
            }
        }

        public HistoryComponent History { get; }

        private void RichTextBox_MouseUp(object sender, MouseEventArgs e) => SyncComponents();

        internal CursorCoordinator(KeywordIndexer keywordIndexer,
                                   DateIndexer dateIndexer,
                                   PageNavigator pageNavigator,
                                   Navigator navigator,
                                   RichTextBox richTextBox,
                                   Outline outline)
        {
            _keywordIndexer = keywordIndexer;
            _keywordIndexer.CursorCoordinator = this;
            _dateIndexer = dateIndexer;
            _dateIndexer.CursorCoordinator = this;
            _pageNavigator = pageNavigator;
            _pageNavigator.CursorCoordinator = this;
            outline.CursorCoordinator = this;
            _navigator = navigator;
            _navigator.CursorCoordinator = this;
            _richTextBox = richTextBox;
            History = new HistoryComponent(_richTextBox);
            On = true;
        }



        /// <summary>
        /// Synchronizes the components 
        /// </summary>
        internal void SyncComponents()
        {
            if (_updatingCursor) return;
            _updatingCursor = true;
            try
            {                
                var selectedRange = _richTextBox.GetSelectedRange();
                UpdateDateIndexer(selectedRange);
                UpdateOtherComponents(selectedRange);
            }
            finally
            {
                _updatingCursor = false;
            }
        }

        private void UpdateOtherComponents(Range selectedRange)
        {
            string keyword = _keywordIndexer.GetKeywordForRange(selectedRange);

            if (keyword != null)
            {
                _keywordIndexer.SelectKeyword(keyword, selectedRange);
                _navigator.SelectKeyword(keyword, selectedRange);
                Range[] ranges = _keywordIndexer.GetRangesForKeyword(keyword);
                DateTime[] dateTimes = _dateIndexer.GetDatesForRanges(ranges);
                _dateIndexer.Calendar.BoldedDates = dateTimes;
            }
            else
            {
                //TODO this is causing problems with next/previous _navigator.SetSelectedRangeIndexNearestTo(selectedRange);
                _dateIndexer.ResetBoldedDaysToDefault();
            }
            _pageNavigator.UpdateVolumeAndPageTextBoxes();
        }

        private void UpdateDateIndexer(Range selectedRange)
        {
            var date = _dateIndexer.GetDateForRange(selectedRange);

            if (date != null)
            {
                try
                {
                    _dateIndexer.Calendar.TodayDate = date.Value;
                    _dateIndexer.Calendar.SetDate(_dateIndexer.Calendar.TodayDate);
                    _dateIndexer.Calendar.Refresh();
                    _dateIndexer.ScrollToRangeInBrowser(selectedRange);
                }
                catch (Exception ex)
                {
                    MessagePanel.Show(_richTextBox, ex.Message, MessagePanel.MessageType.Warning);
                }

            }
        }

        

        private void Calendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            if (_updatingCursor) return;
            _updatingCursor = true;

            var date = e.Start;
            Range range = _dateIndexer.GetRangeForDate(date);

            if (range != null)
            {
                SetCursor(range.Start, 0);
                _dateIndexer.ScrollToRangeInBrowser(range);
                UpdateOtherComponents(range);

            }

            _updatingCursor = false;
        }

        internal void SetCursor(int startIndex, int length)
        {
            _richTextBox.Select(startIndex, length);
            _richTextBox.ScrollToCaret();
            History.AddSelectedRange();
            SyncComponents();
        }

        //private void KeywordIndexer_KeywordSelected(object sender, KeywordSelectedEventArgs e)
        //{
        //    if (_updatingCursor) return;
        //    _updatingCursor = true;

        //    var keyword = e.Keyword;
        //    var range = _keywordIndexer.GetRangeForKeyword(keyword);

        //    if (range != null)
        //    {
        //        _richTextBox.SelectionStart = range.Start;
        //        _richTextBox.SelectionLength = range.Length;
        //        _navigator.SelectKeyword(keyword);
        //    }

        //    _updatingCursor = false;
        //}
    }


}
