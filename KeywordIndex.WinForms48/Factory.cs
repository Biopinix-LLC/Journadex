using Journadex.Library;
using System;

namespace KeywordIndex.WinForms48
{
    internal static class Factory
    {
        internal static IOutlineExporter CreateMarkdownExporter(ExportMode mode) => new MarkdownExporter(mode);

        internal static INodeRenderer CreateHtmlRenderer(Outline outline) => new HtmlRenderer(outline);

        internal static INodeRenderer CreateMarkdownRenderer(Outline outline) => new MarkdownRenderer(outline);

        internal static ITruncateStrategy CreateMessagePanelTruncateStrategy() => new QuoteTruncateStrategy();

        internal static ILinkRenderer CreateHtmlLinkRenderer() => new HtmlRenderer.LinkRenderer();

        internal static IPaginationRenderer CreateHtmlPaginationRenderer()
            => null; // TODO new HtmlRenderer.PaginationRenderer(itemsPerPage: 10);
    }
}