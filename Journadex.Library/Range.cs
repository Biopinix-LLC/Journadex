using Newtonsoft.Json;
using System;

namespace Journadex.Library
{

    public class Range 
    {
        private const char LengthDelimiter = '_';
        private const char EndDelimiter = '-';
        private int _start;
        private int _end;
        [JsonIgnore]
        public string Id => ToRangeString(true); //{ get; } = Guid.NewGuid().ToString(); // TODO split out into separate class

        public Range()
        {
            End = -1;
        }
        public Range(int start, int end)
        {
            _start = start;
            _end = end;
            UpdateLength();
        }


        public int Start
        {
            get { return _start; }
            set
            {
                _start = value;
                UpdateLength();
            }
        }

        public int End
        {
            get { return _end; }
            set
            {
                _end = value;
                UpdateLength();
            }
        }

        public int Length { get; private set; }
        public static int GetEndFromLength(int startIndex, int length) => startIndex + length - 1;

        private void UpdateLength()
        {
            Length = Math.Abs(_end - _start) + 1;
        }

        //public override string ToString() => ToString(useLength: true);
        public string ToRangeString(bool useLength)
        {
            char delimeter;
            int endValue;
            if (useLength)
            {
                delimeter = LengthDelimiter;
                endValue = Length;
            }
            else
            {
                delimeter = EndDelimiter;
                endValue = End;
            }
            return $"{Start}{delimeter}{endValue}";
        }

        public static Range Parse(string str)
        {
            char delimiter;
            bool endIsLength;
            if (str.IndexOf(LengthDelimiter) > -1)
            {
                delimiter = LengthDelimiter;
                endIsLength = true;
            }
            else if (str.IndexOf(EndDelimiter) > -1)
            {
                delimiter = EndDelimiter;
                endIsLength = false;
            }
            else throw new FormatException($"'{str}' must have a '_' or '-' to parse into a range.");
            var parts = str.Split(delimiter);
            if (parts.Length != 2 || !int.TryParse(parts[0], out int start) || !int.TryParse(parts[1], out int endValue)) throw new FormatException($"'{str}' must be two delimited integers to parse into a range.");
            return new Range(start, endIsLength ? GetEndFromLength(start, endValue) : endValue);
        }

        public bool Contains(Range other) => other.Start >= Start && other.Start <= End;

        public static bool TryParse(string rangeString, out Range range)
        {
            if (rangeString.IndexOfAny(new char[] { LengthDelimiter, EndDelimiter }) == -1)
            {
                range = null;
                return false;
            }
            if (string.IsNullOrEmpty(rangeString) || !char.IsDigit(rangeString[0]))
            {
                range = null;
                return false;
            }

            try
            {
                range = Parse(rangeString);
            }
            catch
            {
                range = null;
                return false;
            }
            return true;
        }

        public Range Clone() => new Range(_start, _end);
    }
}