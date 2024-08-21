using Journadex.Library;
using System.Text;
using static Journadex.Library.JournalTree;

namespace Journadex.Winforms
{
    internal class TxtJournalNodeVisitor : JournalNodeVisitor
    {
        private StringBuilder _txt = new StringBuilder();

        public TxtJournalNodeVisitor(JournalTree tree, bool isIndex) : base(tree, isIndex) { }
        public override void Visit(JournalNode node)
        {
            bool asHyperlink = false;
            int indentLevel = GetIndentLevel(node);

            // Create the RTF string for the current node
            _txt.Append(new string('\t', indentLevel)).Append(indentLevel % 2 == 0 ? '*' : '-').Append(' ').Append(GetNodeText(node));
        }

        public override string ToString() => _txt.ToString();
    }
}