using Journadex.Library;
using System.Linq;
using System.Text;
using static KeywordIndex.WinForms48.Outline;

namespace KeywordIndex.WinForms48
{
    internal partial class HtmlRenderer
    {
        internal class PaginationRenderer : IPaginationRenderer
        {
            public int CurrentPage { get; set; } = 1;
            public int ItemsPerPage { get; private set; }
            public PaginationRenderer(int itemsPerPage)
            {
                ItemsPerPage = itemsPerPage;
            }

            public void RenderPagination(StringBuilder builder, Node root)
            {
                int totalPages = CalculateTotalPages(root);

                if (totalPages <= 1)
                {
                    return;
                }

                builder.AppendLine("<div class='pagination'>");

                for (int i = 1; i <= totalPages; i++)
                {
                    builder.AppendFormat("<a href='#page{0}'>Page {0}</a>", i);
                    if (i < totalPages)
                    {
                        builder.Append(" | ");
                    }
                }

                builder.AppendLine("</div>");
            }

            private int CalculateTotalPages(Node root)
            {
                if (root.Children.Count == 0)
                {
                    return 0;
                }

                return (root.Children.Flatten(n => n.Children).Count() + ItemsPerPage - 1) / ItemsPerPage;
            }
        }
    }

}
