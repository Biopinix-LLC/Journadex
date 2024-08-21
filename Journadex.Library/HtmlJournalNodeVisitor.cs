using System.Text;
using static Journadex.Library.JournalTree;

namespace Journadex.Library
{
    public class HtmlJournalNodeVisitor : JournalNodeVisitor
    {
        private StringBuilder _html = new StringBuilder();

        public HtmlJournalNodeVisitor(JournalTree tree, bool isIndex) : base(tree, isIndex)
        {
            _html.Append("<html><head><style>ul {list-style-type: none;}</style></head><body><ul>");
        }

        public override void Visit(JournalNode node)
        {
            bool asHyperlink = false;
            // Calculate the indent level of the current node

            var nodeText = GetNodeText(node);
            // Create the HTML string for the current node
            if (asHyperlink)
            {
                _html.Append("<li>").Append("<a href='\\").Append(nodeText).Append("'>").Append(nodeText).Append("</a></li>");
            }
            else
            {
                _html.Append("<li>").Append(nodeText).Append("</li>");
            }
        }

        public override string ToString()
        {
            _html.Append("</ul></body></html>");
            return _html.ToString();
        }
    }





}
