using static KeywordIndex.WinForms48.Outline;
using System.Text;
using System.Windows.Forms;
using System;
using System.IO;

namespace KeywordIndex.WinForms48
{
    public enum ExportMode
    {
        SaveToFile,
        CopyToClipboard
    }

    public class MarkdownExporter : IOutlineExporter
    {
        private readonly ExportMode _exportMode;
        private Outline _outline;
        private INodeRenderer _renderer;

        public MarkdownExporter(ExportMode exportMode)
        {
            _exportMode = exportMode;
        }

        public void Export(Outline outline)
        {
            if (outline == null) throw new ArgumentNullException(nameof(outline));
            _outline = outline;
            _renderer = Factory.CreateMarkdownRenderer(outline);
            var markdownText = GenerateMarkdown(outline.Root);

            switch (_exportMode)
            {
                case ExportMode.SaveToFile:
                    SaveToFile(markdownText);
                    break;
                case ExportMode.CopyToClipboard:
                    CopyToClipboard(markdownText);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_exportMode), $"Export mode '{_exportMode}' is not supported.");
            }
        }

        private string GenerateMarkdown(Node node)
        {
            StringBuilder sb = new StringBuilder();
            if (node != null)
            {
               _renderer.RenderChildren(sb, node);
            }
            return sb.ToString();
        }

        private void SaveToFile(string markdownText)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Markdown files (*.md)|*.md|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.DefaultExt = "md";
                saveFileDialog.AddExtension = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, markdownText);
                }
            }
        }

        private void CopyToClipboard(string markdownText)
        {
            ClipboardWithMessage.SetText(markdownText, _outline.WebBrowser);
        }

    }
}