using Journadex.Library;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KeywordIndex.WinForms48
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public sealed class CheckBoxSelection 
    {
        


        public CheckBoxSelection(ICheckBoxSelectionOwner owner)
        {
            Owner = owner;
        }
        public IRangeSelectionDestinationProvider DestinationProvider { get; set; }
        public ICheckBoxSelectionOwner Owner { get; }

        //public int Count => _checkboxes.Count;



        public event EventHandler SelectionChanged;
        public void OnCheckClicked(string id)
        {
            DestinationProvider.ToggleSelection(id, Owner);
            SelectionChanged?.Invoke(this, EventArgs.Empty);

        }

       
    }
}