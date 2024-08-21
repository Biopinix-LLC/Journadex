using Journadex.Library;
using static KeywordIndex.WinForms48.Outline;

namespace KeywordIndex.WinForms48
{

    using System;
    using System.Collections.Generic;
    using System.Text;

    public abstract class NodeRenderer : INodeRenderer
    {
        protected Outline Outline;
        protected int ItemsPerPage;
        protected bool PaginationEnabled;

        public NodeRenderer(Outline outline, int itemsPerPage = 0)
        {
            Outline = outline;
            ItemsPerPage = itemsPerPage;
            PaginationEnabled = itemsPerPage > 0;
        }

        public IPaginationRenderer Pagination => null; // TODO

        public List<string> Render()
        {
            List<string> pages = new List<string>();
            var currentPage = new StringBuilder();
            int nodesOnCurrentPage = 0;

            RenderNode(Outline.Root, currentPage, ref nodesOnCurrentPage, pages);

            if (currentPage.Length > 0)
            {
                pages.Add(currentPage.ToString());
            }

            return pages;
        }

        public void RenderChild(StringBuilder builder, Node child)
        {
            throw new NotImplementedException();
        }

        public void RenderChildren(StringBuilder builder, Node node)
        {
            throw new NotImplementedException();
        }

        private void RenderNode(Node node, StringBuilder currentPage, ref int nodesOnCurrentPage, List<string> pages)
        {
            if (PaginationEnabled && nodesOnCurrentPage >= ItemsPerPage)
            {
                pages.Add(currentPage.ToString());
                currentPage.Clear();
                nodesOnCurrentPage = 0;
            }

            Range sourceRange;
            currentPage.AppendLine(Outline.GetNodeText(node, out sourceRange));
            nodesOnCurrentPage++;

            foreach (var child in node.Children)
            {
                RenderNode(child, currentPage, ref nodesOnCurrentPage, pages);
            }
        }
    }
}
