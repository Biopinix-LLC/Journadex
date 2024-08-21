namespace Journadex.Library
{
    public interface ITruncateStrategy
    {
        string Truncate(string input, int maxLength, bool useEllipsis);
    }

}