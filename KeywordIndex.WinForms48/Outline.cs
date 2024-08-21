using Journadex.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    public partial class Outline : Indexer, IOutlineFileComponent, IRangeSelectionDestinationProvider
    {
        internal const string EditRefPrefix = "edit_";
        private readonly NodeActions _actions;
        private OutlineContextMenuHandler _contextMenu;
        private readonly INodeRenderer _renderer;

        public Node Root { get; private set; }

        public Outline(RichTextBox richTextBox, WebBrowser webBrowser) : base(richTextBox, webBrowser)
        {
            Root = new Node();
            _actions = new NodeActions(this);
            _contextMenu = new OutlineContextMenuHandler(webBrowser, _actions);
            _renderer = Factory.CreateHtmlRenderer(this);
        }


        protected override void BuildHtmlBody(StringBuilder html) => _renderer.RenderChildren(html, Root);

        

        

        internal string GetNodeText(Node child, out Range sourceRange)
        {
            string result = child.Notes;
            
            if (child.Source == null)
            {
                sourceRange = null;
                return result;
            }
            string sourceText = GetSourceText(child, out sourceRange);

            if (string.IsNullOrWhiteSpace(result))
            {
                return sourceText;
            }
            return result;
        }


        private string GetSourceText(Node child, out Range sourceRange)
        {
            string nodeText;
            if (child.IsSourceUserEdited)
            {
                sourceRange = child.Source;
                nodeText = sourceRange.Substring(RichTextBox.Text);
            }
            else
            {
                nodeText = GetSentence(child, out sourceRange);
                child.VisibleSource = sourceRange;
            }
            return nodeText;
        }

        private string GetSentence(Node child, out Range sentanceRange) => RichTextBox.GetSentence(child.IndexSource, out sentanceRange);

        protected override void ParseUrlFragment(string fragment)
        {
            if (_renderer?.Pagination != null && fragment.StartsWith("page"))
            {               
                _renderer.Pagination.CurrentPage = int.Parse(fragment.Substring("page".Length));
                UpdateIndex();
                return;
            }
            if (fragment.StartsWith(EditRefPrefix))
            {
                _actions.Edit(id: fragment.Substring(EditRefPrefix.Length));
                return;
            }
            base.ParseUrlFragment(fragment);
        }



        protected override void RenderRange(StringBuilder html, Range index)
            => throw new NotSupportedException($"Call {nameof(_renderer.RenderChildren)} instead.");

        public void LoadFromOutline(IOutlineData outlineData)
        {
            if (outlineData.Outline?.Root != null)
            {
                Root = outlineData.Outline.Root;
                UpdateIndex();
            }
        }

        public void SaveToOutline(IOutlineData outlineData) => outlineData.Outline = new OutlineInfo { Root = Root };
        internal Range[] GetRanges(bool fromDateIndex) // TODO this doesn't seem to work anymore
            => GetAllNodesWithSources(fromDateIndex).Select(node => node.IndexSource).ToArray();

        internal Node[] FindNodesInRange(Range range, bool fromDateIndex)
        {
            return GetAllNodesWithSources(fromDateIndex).Where(node => range != null && range.Contains(node.IndexSource)).ToArray();
        }

        //internal Range[] UpdateFromIndex(Indexer indexer, bool fromDateIndex)
        //{
        //    var selectedRanges = from kvp in _checkboxes
        //                         where kvp.Value.SelectionOwner == indexer
        //                         select kvp.Value.Range;
        //    List<Range> keepSelected = new List<Range>();
        //    // Add new nodes for the selected ranges if they don't already exist
        //    var existingRangeSources = GetAllNodesWithSources(fromDateIndex).Select(node => node.IndexSource).ToArray();
        //    var rangesToAdd = selectedRanges.Where(range => !existingRangeSources.Any(r => r.Start == range.Start && r.End == range.End)).ToArray();
        //    foreach (Range range in rangesToAdd)
        //    {
        //        AddNodeFromIndex(range, fromDateIndex);
        //    }

        //    // Remove nodes with sources that don't match any of the selected ranges
        //    var nodesToRemove = GetAllNodesWithSources(fromDateIndex).Where(node => !selectedRanges.Any(r => r.Start == node.IndexSource.Start && r.End == node.IndexSource.End)).ToArray();
        //    int skipCount = -1;
        //    foreach (var node in nodesToRemove)
        //    {
        //        skipCount++;
        //         if (!RemoveNode(node, fromDateIndex, out bool cancel))
        //        {
        //            if (cancel)
        //            {
        //                keepSelected.AddRange(nodesToRemove.Skip(skipCount).Select(n => n.IndexSource));
        //                break;
        //            }
        //            keepSelected.Add(node.IndexSource);
        //            continue;
        //        }

        //    }
        //    UpdateIndex();
        //    return keepSelected.ToArray();
        //}


        private bool RemoveNode(Node node, bool fromDateIndex, out bool cancel)
        {
            Node parent = node.Parent ?? Root;
            // ChatGPT: check if any children of the current node don't have any source or have notes and warn the user that this information will be lost if they continue.
            bool hasNotes = !string.IsNullOrEmpty(node.Notes) || node.Children.Any(n => !string.IsNullOrEmpty(n.Notes));
            bool hasNoSources = node.Children.Any(n => n.IndexSource == null);
            //skipCount++;
            if (hasNotes || hasNoSources)
            {
                string warning = "Warning: Removing this node will also remove its children, which";
                warning += hasNotes ? " have notes" : "";
                warning += hasNoSources ? " and/or are missing sources" : "";
                warning += ". Continue?";
                if (!Confirm(warning, out cancel))
                {
                    return false;
                }

            }
            parent.Children.Remove(node);
            cancel = false;
            return true;
        }

        private void AddNodeFromIndex(Range range, bool fromDateIndex)
        {
            // Create a new node with the source set to the range
            Node parent = FindNodesInRange(range, fromDateIndex).FirstOrDefault() ?? Root;
            var newNode = new Node(parent, indexSource: range, isSourceDateIndex: fromDateIndex);
            // Add the new node to the tree
            parent.Children.Add(newNode);
        }

        private IEnumerable<Node> GetAllNodesWithSources(bool fromDateIndex)
        {
            return GetAllNodesWithSources().Where(node => node.IsSourceDateIndex == fromDateIndex);
        }

        private IEnumerable<Node> GetAllNodesWithSources()
        {
            return Root.Children.Flatten(node => node.Children).Where(node => node.IndexSource != null);
        }

        private bool Confirm(string warning, out bool cancel)
        {
            DialogResult dialogResult = MessageBox.Show(warning, "Warning", buttons: MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, defaultButton: MessageBoxDefaultButton.Button2);
            cancel = dialogResult == DialogResult.Cancel;
            return dialogResult == DialogResult.Yes;
        }



        void IRangeSelectionDestinationProvider.ToggleSelection(string indexSourceId, ICheckBoxSelectionOwner selectionOwner)
        {

            var node = GetNodeOfIndexSource(indexSourceId);
            if (node == null)
            {
                AddSelectedRangeById(indexSourceId, selectionOwner);
                return;
            }
            RemoveSelectedRange(node.IndexSource, selectionOwner);


        }

        private void RemoveSelectedRange(Range indexSource, ICheckBoxSelectionOwner selectionOwner)
        {
            RemoveNode(GetNodeOfIndexSource(indexSource.Id), selectionOwner.IsDateIndex, out _);
            UpdateIndex();
        }

        private void AddSelectedRangeById(string indexSourceId, ICheckBoxSelectionOwner selectionOwner)
        {
            AddNodeFromIndex(selectionOwner.GetRangeById(indexSourceId), selectionOwner.IsDateIndex);
            UpdateIndex();
        }

        public bool IsSelected(string indexSourceId) => GetNodeOfIndexSource(indexSourceId) != null;

        private Node GetNodeOfIndexSource(string indexSourceId)
        {
            return GetAllNodesWithSources().FirstOrDefault(n => n.IndexSource.Id == indexSourceId);
        }


        //private void Add(string id, Range index) => _checkboxes.Add(id, new CheckedRange(index));

        internal void AddSelectedRanges(Range[] ranges, ICheckBoxSelectionOwner selectionOwner)
        {
            foreach (var range in ranges)
            {
                AddSelectedRangeById(range.Id, selectionOwner);
            }
        }



        //internal string GetOrAddId(Range range)
        //{
        //    var id = range.ToRangeString(useLength: true);
        //    if (!_checkboxes.ContainsKey(id))
        //    {
        //        Add(id, range);
        //    }
        //    return id;
        //}

        void IRangeSelectionDestinationProvider.AddSelectedItems(Range[] items, ICheckBoxSelectionOwner selectionOwner)
        {
            AddSelectedRanges(items, selectionOwner);
        }


        public class OutlineInfo
        {
            public Node Root { get; set; }
        }
    }
}
