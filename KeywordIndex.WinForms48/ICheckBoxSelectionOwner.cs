using Journadex.Library;

namespace KeywordIndex.WinForms48
{
    public interface ICheckBoxSelectionOwner
    {
        bool IsDateIndex { get; }

        Range GetRangeById(string indexSourceId);
    }
}