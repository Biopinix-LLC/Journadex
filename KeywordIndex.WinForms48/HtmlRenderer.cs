using Journadex.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KeywordIndex.WinForms48.Outline;

namespace KeywordIndex.WinForms48
{
    internal partial class HtmlRenderer : BaseNodeRenderer
    {
        public HtmlRenderer(Outline outline) : base(outline,
                                                    Factory.CreateHtmlLinkRenderer()
                                                    ,Factory.CreateHtmlPaginationRenderer() // TODO not working yet... debug
            )
        { }

        protected override void BeginList(StringBuilder html, Node node)
        {
            if (node.NumberedChildren)
            {
                html.Append("<ol");
                if (node.StartAtNumber > 1)
                {
                    html.Append(" start=\"").Append(node.StartAtNumber).Append('"');
                }
                html.Append('>');
            }
            else
            {
                html.Append("<ul>");
            }
        }

        protected override void EndList(StringBuilder html, Node node)
        {
            html.Append(node.NumberedChildren ? "</ol>" : "</ul>");
        }

        protected override void EndChild(StringBuilder builder)
        {
            builder.Append("</li>");
        }


        protected override void StartChild(StringBuilder builder)
        {
            builder.Append("<li>");
        }

        internal class LinkRenderer : ILinkRenderer
        {
            public void RenderChildEditLink(StringBuilder builder, Node child)
            {
                builder.Append(" <a href=#").Append(EditRefPrefix).Append(child.Id).Append(">Edit</a>");
            }

            public void RenderChildSourceLink(StringBuilder builder, Node child, Range sourceRange)
            {
                if (child.Source != null)
                {
                    if (sourceRange != null)
                    {
                        builder.Append(" <a href=#").Append(BaseNodeRenderer.GetSourceRangeString(sourceRange)).Append(">Source</a>");
                    }
                    else
                    {
                        builder.Append("<i>Source Missing</i>");
                    }
                }
            }
        }
    }

}
