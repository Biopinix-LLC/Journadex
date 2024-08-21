using static Journadex.Library.JournalTree;

namespace Journadex.Library
{
    public abstract class JournalNodeVisitor
    {
        public JournalNodeVisitor(JournalTree tree, bool isIndex)
        {
            Tree = tree;
            IsIndex = isIndex;
        }

        
        protected JournalTree Tree { get; }
        public bool IsIndex { get; }

        public abstract void Visit(JournalNode node);

        protected string GetNodeText(JournalNode currentNode)
        {
            return currentNode.Type == NodeType.Root ? "Journal" : Tree.Text.Substring(currentNode.Range);
            // TODO get text from root using node and strategy
        }

        protected int GetIndentLevel(JournalNode node)
        {
            // Calculate the indent level of the current node
            int indentLevel = 0;
            JournalNode currentNode = node;
            while (currentNode.Parent != null)
            {
                indentLevel++;
                currentNode = currentNode.Parent;
            }

            return indentLevel;
        }
    }

            
  

}
