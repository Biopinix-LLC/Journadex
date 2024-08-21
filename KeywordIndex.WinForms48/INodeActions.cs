namespace KeywordIndex.WinForms48
{
    internal enum MoveActions { Top, Up, Down, Bottom }
    internal interface INodeActions
    {        
        void AddChild(string id);
        void AddParent(string id);
        void AddSibling(string id, bool above);
        void ChangeStartingNumber(string id);
        void Delete(string id, bool prompt);
        void Edit(string id);
        void Export(IOutlineExporter exporter);
        Outline.Node GetValidNode(string id);
        void Move(string id, MoveActions actions);
        void PasteAsChild(string id, Outline.Node _copy);
        void PasteAsParent(string id, Outline.Node _copy);
        void PasteAsSibling(string id, Outline.Node _copy, bool above);
        void ToggleChildrenType(string id);
    }
}