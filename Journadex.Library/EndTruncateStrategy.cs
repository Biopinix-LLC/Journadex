namespace Journadex.Library
{
    public class EndTruncateStrategy : TruncateStrategyBase
    {
        public override string Truncate(string input, int maxLength, bool useEllipsis)
        {
            if (string.IsNullOrEmpty(input) || maxLength <= 0 || input.Length <= maxLength)
            {
                return input;
            }

            string ellipsis = GetEllipsis(useEllipsis);
            int ellipsisLength = ellipsis.Length;

            return input.Substring(0, maxLength - ellipsisLength) + ellipsis;
        }
    }

}