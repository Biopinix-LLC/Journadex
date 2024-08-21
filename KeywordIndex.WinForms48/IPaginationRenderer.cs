using System.Text;
using static KeywordIndex.WinForms48.Outline;

namespace KeywordIndex.WinForms48
{
    public interface IPaginationRenderer
    {
        int CurrentPage { get; set; }
        int ItemsPerPage { get; }

        void RenderPagination(StringBuilder builder, Node root);
    }

}
