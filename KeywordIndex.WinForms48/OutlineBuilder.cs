using Journadex.Library;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    public class OutlineBuilder
    {
        private RichTextBox _textBox;
        private List<Range> _ranges;
        private StringBuilder _outlineBuilder;

        public OutlineBuilder(RichTextBox textBox)
        {
            _textBox = textBox;
        }

        public OutlineBuilder WithRanges(Range[] ranges)
        {
            _ranges = ranges.ToList();

            return this;
        }
        public OutlineBuilder BuildOutline()
        {
             _outlineBuilder = new StringBuilder();

            if (_ranges.Count == 0) return this;
            _outlineBuilder.Append("{\\rtf1\\ansi\n");

            // Sort the ranges in order by their start position
            _ranges.Sort((r1, r2) => r1.Start.CompareTo(r2.Start));

            foreach (Range range in _ranges)
            {
                string text = _textBox.Text.Substring(range.Start, range.Length);
                _outlineBuilder.Append("\\bullet " + text + "\\par\n");
            }

            _outlineBuilder.Append("}");

            return this;
        }


        public string GetOutline()
        {
            return _outlineBuilder.ToString();
        }
    }
}
