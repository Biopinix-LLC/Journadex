using Journadex.Library;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KeywordIndex.WinForms48.Outline;

namespace KeywordIndex.WinForms48
{
    internal class MarkdownRenderer : BaseNodeRenderer
    {
        private int _indent;

        public MarkdownRenderer(Outline outline) : base(outline) { }

        protected override void BeginList(StringBuilder markdown, Node node)
        {
            _indent++;
        }

        protected override void EndChild(StringBuilder builder)
        {
            builder.AppendLine();
        }

        protected override void EndList(StringBuilder markdown, Node node)
        {
            _indent--;
            // Add a newline after a list in Markdown
            markdown.AppendLine();
        }

       
        protected override void StartChild(StringBuilder builder)
        {
            builder.Append(new string(' ', _indent * 2)).Append("- ");
        }
    }

}
