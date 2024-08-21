using System.Collections.Generic;
namespace KeywordIndex.WinForms48
{
    public partial class KeywordIndexer
    {
        public class KeywordInfo
        {
            public Dictionary<string, IndexItemInfo> Index { get; set; }
            public Dictionary<string, string> Aliases { get; set; }
        }
    }
}
