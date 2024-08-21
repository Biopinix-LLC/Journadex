namespace Journadex.Library
{
    public class MiddleTruncateStrategy : TruncateStrategyBase
    {
        public override string Truncate(string input, int maxLength, bool useEllipsis)
        {
            if (string.IsNullOrEmpty(input) || maxLength <= 0 || input.Length <= maxLength)
            {
                return input;
            }

            string ellipsis = GetEllipsis(useEllipsis);
            int ellipsisLength = ellipsis.Length;

            int firstHalfLength = (maxLength - ellipsisLength) / 2;
            int secondHalfLength = maxLength - firstHalfLength - ellipsisLength;

            return input.Substring(0, firstHalfLength) + ellipsis + input.Substring(input.Length - secondHalfLength);
        }
    }

}