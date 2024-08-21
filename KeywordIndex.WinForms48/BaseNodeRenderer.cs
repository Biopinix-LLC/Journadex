using Journadex.Library;
using System.Linq;
using static KeywordIndex.WinForms48.HtmlRenderer;
using static KeywordIndex.WinForms48.Outline;

namespace KeywordIndex.WinForms48
{
    using System.Text;

    internal abstract class BaseNodeRenderer : INodeRenderer
    {
        protected readonly Outline _outline;
        protected readonly ILinkRenderer _linkRenderer;
        private int _renderedNodesCount;
        private int _nodesToSkip;

        public IPaginationRenderer Pagination { get; }

        protected BaseNodeRenderer(Outline outline, ILinkRenderer linkRenderer = null, IPaginationRenderer paginationRenderer = null)
        {
            _outline = outline;
            _linkRenderer = linkRenderer;
            Pagination = paginationRenderer;
        }



        public void RenderChildren(StringBuilder builder, Node node)
        {
            if (node == null) return;
            if (node.Children.Count == 0) return;

            if (Pagination?.ItemsPerPage > 0)
            {
                int nodesToSkip = (Pagination.CurrentPage - 1) * Pagination.ItemsPerPage;
                RenderChildrenWithPagination(builder, node, ref nodesToSkip);
            }
            else
            {
                BeginList(builder, node);

                foreach (var child in node.Children)
                {
                    if (child == null) continue;
                    if (child.Parent == null) child.SetParent(node);
                    RenderChild(builder, child);
                }

                EndList(builder, node); 
            }
            if (Pagination != null && node == _outline.Root)
            {
                Pagination.RenderPagination(builder, _outline.Root);
            }
        }




        private void RenderChildrenWithPagination(StringBuilder builder, Node node, ref int nodesToSkip, int renderedNodesCount = 0)
        {
            if (node == null) return;
            if (node.Children.Count == 0) return;

            BeginList(builder, node);

            foreach (var child in node.Children)
            {
                if (child == null) continue;
                if (child.Parent == null) child.SetParent(node);

                if (nodesToSkip > 0)
                {
                    RenderChildrenWithPagination(builder, child, ref nodesToSkip, renderedNodesCount);
                    nodesToSkip--;
                    continue;
                }

                if (renderedNodesCount < Pagination.ItemsPerPage)
                {
                    renderedNodesCount++;
                    RenderChild(builder, child);

                    RenderChildrenWithPagination(builder, child, ref nodesToSkip, renderedNodesCount);
                }
                else
                {
                    break;
                }
            }

            EndList(builder, node);
        }


        public void RenderChild(StringBuilder builder, Node child)
        {
            Range sourceRange;
            string nodeText = GetNodeText(child, out sourceRange);
            if (!string.IsNullOrWhiteSpace(nodeText))
            {
                StartChild(builder);
                builder.Append(nodeText);
                if (_linkRenderer != null)
                {
                    _linkRenderer.RenderChildSourceLink(builder, child, sourceRange);
                    _linkRenderer.RenderChildEditLink(builder, child);
                }
                EndChild(builder);
            }
            RenderChildren(builder, child);
        }

        protected virtual string GetNodeText(Node child, out Range sourceRange) => _outline.GetNodeText(child, out sourceRange)?.RemoveNewLines();

        protected abstract void BeginList(StringBuilder builder, Node node);
        protected abstract void EndList(StringBuilder builder, Node node);
        protected abstract void EndChild(StringBuilder builder);
        protected abstract void StartChild(StringBuilder builder);
        internal static string GetSourceRangeString(Range sourceRange)
        {
            return sourceRange.ToRangeString(useLength: true);
        }


    }
}
