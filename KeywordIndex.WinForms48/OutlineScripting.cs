namespace KeywordIndex.WinForms48
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public class OutlineScripting
    {
        public OutlineScripting(Outline owner)
        {
            Owner = owner;
        }

        internal Outline Owner { get; }

        public void OnTextChanging(string id, string value)
        {
            Owner.SetTextById(id, value);
        }
    }
}