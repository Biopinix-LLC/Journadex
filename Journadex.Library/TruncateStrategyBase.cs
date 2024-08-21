namespace Journadex.Library
{
    public abstract class TruncateStrategyBase : ITruncateStrategy
    {
        public abstract string Truncate(string input, int maxLength, bool useEllipsis);

        protected string GetEllipsis(bool useEllipsis)
        {
            return useEllipsis ? "..." : "";
        }
    }

}