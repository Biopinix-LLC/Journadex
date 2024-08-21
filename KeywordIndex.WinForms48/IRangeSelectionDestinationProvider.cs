using Journadex.Library;

namespace KeywordIndex.WinForms48
{
    public interface IRangeSelectionDestinationProvider
    {
        //Range[] SelectedItems { get; }        

        bool IsSelected(string id);
        void ToggleSelection(string id, ICheckBoxSelectionOwner selectionOwner);
        void AddSelectedItems(Range[] items, ICheckBoxSelectionOwner selectionOwner);
    }
}