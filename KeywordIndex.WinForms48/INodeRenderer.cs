using System.Text;

namespace KeywordIndex.WinForms48
{
    internal interface INodeRenderer
    {
        IPaginationRenderer Pagination { get; }

        void RenderChild(StringBuilder builder, Outline.Node child);
        void RenderChildren(StringBuilder builder, Outline.Node node);
    }
}