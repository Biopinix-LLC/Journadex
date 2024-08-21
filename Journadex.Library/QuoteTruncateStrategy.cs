namespace Journadex.Library
{
    public class QuoteTruncateStrategy : MiddleTruncateStrategy
    {
        public override string Truncate(string input, int maxLength, bool useEllipsis)
        {
            if (string.IsNullOrEmpty(input) || maxLength <= 0 || input.Length <= maxLength)
            {
                return input;
            }

            string ellipsis = GetEllipsis(useEllipsis);
            int ellipsisLength = ellipsis.Length;

            int quoteStartIndex = input.IndexOf('"');
            int quoteEndIndex = input.LastIndexOf('"');

            if (quoteStartIndex < 0 || quoteEndIndex <= quoteStartIndex)
            {
                return base.Truncate(input, maxLength, useEllipsis);
            }
            int quotedStringLength = quoteEndIndex - quoteStartIndex - 1;
            if (quotedStringLength > maxLength)
            {
                int firstHalfLength = (maxLength - ellipsisLength) / 2;
                int secondHalfLength = maxLength - firstHalfLength - ellipsisLength;

                string beforeQuote = input.Substring(0, quoteStartIndex + 1);
                string quotedString = input.Substring(quoteStartIndex + 1, firstHalfLength) + ellipsis + input.Substring(quoteEndIndex - secondHalfLength, secondHalfLength);
                string afterQuote = input.Substring(quoteEndIndex);

                return beforeQuote + quotedString.Trim() + afterQuote;
            }

            return input;
        }
    }


}