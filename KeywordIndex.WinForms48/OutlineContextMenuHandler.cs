using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static KeywordIndex.WinForms48.Outline;

namespace KeywordIndex.WinForms48
{
    internal class OutlineContextMenuHandler : WebBrowserContextMenuHandler
    {
        private readonly INodeActions _actions;
        private Node _copy;

        public OutlineContextMenuHandler(WebBrowser webBrowser, INodeActions actions) : base(webBrowser)
        {
            _actions = actions;
            

        }

        protected override bool PopulateMenu(ContextMenuStrip contextMenu)
        {
            contextMenu.Items.Add("Edit", image: null, OnEditClick);
            PopulateExportMenu(contextMenu);
            var node = _actions.GetValidNode(CurrentId);
            PopulateMoveMenu(contextMenu);
            contextMenu.Items.Add("Delete", image: null, OnDeleteClick);
            contextMenu.Items.Add("-");
            if (node.NumberedChildren)
            {
                contextMenu.Items.Add("Change Children to Bullets", image: null, OnToggleChildrenType);
                contextMenu.Items.Add("Change Starting Number...", image: null, OnChangeStartingNumber);
            }
            else
            {
                contextMenu.Items.Add("Change Children to Numbers", image: null, OnToggleChildrenType);
            }
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Add Child", image: null, OnAddChildClick);
            var addSiblingMenu = new ToolStripMenuItem("Add Sibling", image: null);
            addSiblingMenu.DropDownItems.Add("Below", image: null, (s, e) => _actions.AddSibling(CurrentId, above: false));
            addSiblingMenu.DropDownItems.Add("Above", image: null, (s, e) => _actions.AddSibling(CurrentId, above: true));
            contextMenu.Items.Add(addSiblingMenu);
            contextMenu.Items.Add("Add Parent", image: null, OnAddParentClick);
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Cut", image: null, OnCutClick);
            contextMenu.Items.Add("Copy", image: null, OnCopyClick);
            if (_copy != null)
            {
                var pasteSiblingMenu = new ToolStripMenuItem("Paste as Sibling", image: null);
                pasteSiblingMenu.DropDownItems.Add("Below", image: null, (s, e) => _actions.PasteAsSibling(CurrentId, _copy, above: false));
                pasteSiblingMenu.DropDownItems.Add("Above", image: null, (s, e) => _actions.PasteAsSibling(CurrentId, _copy, above: true));
                contextMenu.Items.Add(pasteSiblingMenu);
                contextMenu.Items.Add("Paste as Child", image: null, OnPasteAsChildClick);
                contextMenu.Items.Add("Paste as Parent", image: null, OnPasteAsParentClick);
            }
            contextMenu.Items.Add("-");
            return true;
        }

        private void PopulateExportMenu(ContextMenuStrip contextMenuStrip)
        {
            var exportMenuItem = new ToolStripMenuItem("Export To");

            var toMarkdownMenuItem = new ToolStripMenuItem("Markdown");
            var toMarkdownFileMenuItem = new ToolStripMenuItem("To File");
            toMarkdownFileMenuItem.Click += ToMarkdownFileMenuItemClick;
            toMarkdownMenuItem.DropDownItems.Add(toMarkdownFileMenuItem);

            var toMarkdownClipboardMenuItem = new ToolStripMenuItem("To Clipboard");
            toMarkdownClipboardMenuItem.Click += ToMarkdownClipboardMenuItemClick;
            toMarkdownMenuItem.DropDownItems.Add(toMarkdownClipboardMenuItem);

            exportMenuItem.DropDownItems.Add(toMarkdownMenuItem);

            contextMenuStrip.Items.Add(exportMenuItem);
        }

        private void ToMarkdownFileMenuItemClick(object sender, EventArgs e)
        {
            _actions.Export(Factory.CreateMarkdownExporter(ExportMode.SaveToFile));
        }

        private void ToMarkdownClipboardMenuItemClick(object sender, EventArgs e)
        {
            _actions.Export(Factory.CreateMarkdownExporter(ExportMode.CopyToClipboard));
        }


        /// <summary>
        /// ChatGPT: Write a method that will add a Move item with subitems for "To Top", "Up", "Down", "To Bottom" and click handlers that will set the node to the custom order.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="contextMenuStrip"></param>
        public void PopulateMoveMenu(ContextMenuStrip contextMenuStrip)
        {
            var moveMenuItem = new ToolStripMenuItem("Move");

            var toTopMenuItem = new ToolStripMenuItem("To Top");
            toTopMenuItem.Click += ToTopMenuItemClick;
            moveMenuItem.DropDownItems.Add(toTopMenuItem);

            var upMenuItem = new ToolStripMenuItem("Up");
            upMenuItem.Click += UpMenuItemClick;
            moveMenuItem.DropDownItems.Add(upMenuItem);

            var downMenuItem = new ToolStripMenuItem("Down");
            downMenuItem.Click += DownMenuItemClick;
            moveMenuItem.DropDownItems.Add(downMenuItem);

            var toBottomMenuItem = new ToolStripMenuItem("To Bottom");
            toBottomMenuItem.Click += ToBottomMenuItemClick;
            moveMenuItem.DropDownItems.Add(toBottomMenuItem);

            contextMenuStrip.Items.Add(moveMenuItem);
        }

        private void ToBottomMenuItemClick(object sender, EventArgs e) => _actions.Move(CurrentId, MoveActions.Bottom);

        private void DownMenuItemClick(object sender, EventArgs e) => _actions.Move(CurrentId, MoveActions.Down);

        private void UpMenuItemClick(object sender, EventArgs e) => _actions.Move(CurrentId, MoveActions.Up);

        private void ToTopMenuItemClick(object sender, EventArgs e) => _actions.Move(id: CurrentId, MoveActions.Top);

        private void OnChangeStartingNumber(object sender, EventArgs e)
        {
            _actions.ChangeStartingNumber(id: CurrentId);
        }

        private void OnToggleChildrenType(object sender, EventArgs e) => _actions.ToggleChildrenType(id: CurrentId);

        private void OnCutClick(object sender, EventArgs e)
        {
            Copy();
            _actions.Delete(CurrentId, prompt: false);
        }

        private void OnPasteAsParentClick(object sender, EventArgs e) => _actions.PasteAsParent(id: CurrentId, _copy);

        private void OnPasteAsChildClick(object sender, EventArgs e) => _actions.PasteAsChild(id: CurrentId, _copy);

        private void OnCopyClick(object sender, EventArgs e) => Copy();

        private void Copy() => _copy = _actions.GetValidNode(CurrentId);

        private void OnAddParentClick(object sender, EventArgs e) => _actions.AddParent(id: CurrentId);

        private void OnAddChildClick(object sender, EventArgs e) => _actions.AddChild(id: CurrentId);


        private void OnEditClick(object sender, EventArgs e) => _actions.Edit(id: CurrentId);

        private void OnDeleteClick(object sender, EventArgs e) => _actions.Delete(id: CurrentId, prompt: true);
    }
}
