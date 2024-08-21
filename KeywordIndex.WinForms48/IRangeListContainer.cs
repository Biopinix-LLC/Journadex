using Journadex.Library;
using System;
using System.Collections.Generic;

namespace KeywordIndex.WinForms48
{
    internal interface IRangeListContainer
    {
        event EventHandler<RangeListChangedEventArgs> IndexChanged;
        Dictionary<string, List<Range>> GetIndex();
    }
}