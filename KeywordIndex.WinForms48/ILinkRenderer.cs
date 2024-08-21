using Journadex.Library;
using System.Text;
using static KeywordIndex.WinForms48.Outline;

namespace KeywordIndex.WinForms48
{
    public interface ILinkRenderer
    {
        void RenderChildEditLink(StringBuilder builder, Node child);
        void RenderChildSourceLink(StringBuilder builder, Node child, Range sourceRange);
    }
}
