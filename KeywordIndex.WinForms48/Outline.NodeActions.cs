using Journadex.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    public partial class Outline
    {
        private class NodeActions : INodeActions
        {
            private readonly Outline _owner;

            public NodeActions(Outline owner)
            {
                _owner = owner;
            }

            private Node GetNode(string id) => _owner.Root.Children.Flatten(node => node.Children).FirstOrDefault(node => node.Id == id);
            public void PasteAsChild(string id, Node copy)
            {
                var parent = GetValidNode(id);
                Node item = copy.Clone();
                item.SetParent(parent);
                parent.Children.Add(item);
                _owner.UpdateIndex();
            }

            public void PasteAsParent(string id, Node copy)
            {
                var node = GetValidNode(id);
                var grandparent = GetParent(node);
                Node parent = copy.Clone();
                grandparent.Children.Add(parent);
                node.Move(grandparent, parent);
                _owner.UpdateIndex();
            }

            public void PasteAsSibling(string id, Node copy, bool pasteAbove = false)
            {
                var node = GetValidNode(id);
                Node sibling = copy.Clone();
                node.AddSibling(sibling, pasteAbove, _owner.Root);
                _owner.UpdateIndex();
            }
            public void AddChild(string id)
            {
                var parent = GetValidNode(id);
                Node node = new Node(parent);
                if (ShowEditNotes(node, "Add Child"))
                {
                    parent.Children.Add(node);
                    _owner.UpdateIndex();
                }

            }

            public void AddParent(string id)
            {
                var node = GetValidNode(id);
                Node grandparent = GetParent(node);
                Node parent = new Node(grandparent);
                if (ShowEditNotes(parent, "Add Parent"))
                {
                    grandparent.Children.Add(parent);
                    node.Move(grandparent, parent);
                    _owner.UpdateIndex();
                }
            }

            private Node GetParent(Node node) => node.Parent ?? _owner.Root;

            public void AddSibling(string id, bool above)
            {
                var node = GetValidNode(id);
                Node parent = GetParent(node);
                Node sibling = new Node(parent);
                if (ShowEditNotes(sibling, "Add Sibling"))
                {
                    node.AddSibling(sibling, above, _owner.Root);
                    _owner.UpdateIndex();
                }
            }

            public void Delete(string id, bool prompt)
            {
                var node = GetValidNode(id);
                bool delete = true;
                if (prompt)
                {
                    StringBuilder sb = new StringBuilder(64);
                    sb.Append("Are you sure you want to delete \"").Append(GetNodeText(node)).Append('"');
                    int childrenCount = node.Children.Flatten(n => n.Children).Count();
                    if (childrenCount > 0)
                    {
                        sb.Append(" and ");
                        if (childrenCount > 1)
                        {
                            sb.Append("all ").Append(childrenCount).Append(" of its children");
                        }
                        else
                        {
                            sb.Append("its child");
                        }
                    }
                    sb.Append('?');
                    delete = MessageBox.Show(sb.ToString(), "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
                }
                if (delete)
                { 
                    var parent = GetParent(node);
                    parent.Children.Remove(node);
                    _owner.UpdateIndex();
                }
            }

            public void Edit(string id)
            {
                Node node = GetValidNode(id);
                // TODO if the node text is a sentance the outline should reflect that source and the index source should be unchanged
                string defaultValue = GetNodeText(node);
                if (ShowEditNotes(node, defaultValue: defaultValue, showRanges: true))
                {
                    _owner.UpdateIndex();
                }
            }

            private string GetNodeText(Node node)
            {
                return string.IsNullOrWhiteSpace(node.Notes) ? _owner.GetSentence(node, out _) : node.Notes;
            }

            public Node GetValidNode(string id)
            {
                if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
                Node node = GetNode(id);
                if (node == null) throw new ArgumentOutOfRangeException($"Node with id '{id}' not found.");
                return node;
            }

            private bool ShowEditNotes(Node node, string caption = "Edit Notes", string defaultValue = null, bool showRanges = false)
            {
                Control[] flowControls;
                if (showRanges)
                {
                    Button rangesButton = CreateRangesButton(node);
                    flowControls = new Control[] { rangesButton };
                }
                else
                {
                    flowControls = null;
                }

                var result = InputDialog<string>.ShowDialog(caption, "Notes", defaultValue, items: null, options: null, flowControls);
                if (result != null && result.Item1 is string notes && (string.IsNullOrEmpty(notes) || notes != defaultValue))
                {
                    node.Notes = notes;
                    return true;
                }
                return false;
            }

            private Button CreateRangesButton(Node node)
            {
                Button rangesButton = new Button
                {
                    AutoSize = true,
                    Text = "Sources"
                };
                rangesButton.Click += (object sender, EventArgs e) =>
                {
                    (sender as Button).FindForm().Close();

                    _owner.RichTextBox.Select(node.Source.Start, node.Source.Length);

                    List<Range> ranges = new List<Range>();
                    Button okButton = new Button
                    {
                        AutoSize = true,
                        Text = "OK",
                    };
                    okButton.Click += (object s, EventArgs re) =>
                    {
                        Range selectedRange = _owner.RichTextBox.GetSelectedRange();
                        // TODO should you be able to edit the initial selection and add new child selections at the same time? How can we differentiate between the initial selected range and the ones added after?
                        if (ranges.Count > 0)
                        {
                            foreach (var newRange in ranges)
                            {
                                Node child = new Node(node, newRange) { IsSourceUserEdited = true };
                                node.Children.Add(child);
                            }
                            _owner.UpdateIndex();
                        }
                        else if (selectedRange.Start != node.Source.Start || selectedRange.End != node.Source.End)
                        {
                            node.Source.Start = selectedRange.Start;
                            node.Source.End = selectedRange.End;
                            node.IsSourceUserEdited = true;
                            _owner.UpdateIndex();

                        }

                        RemoveMessagePanelUsingChildFlowButton(s);

                    };
                    Button cancelButton = new Button
                    {
                        AutoSize = true,
                        Text = "Cancel",
                    };

                    cancelButton.Click += (object s, EventArgs re) =>
                    {
                        RemoveMessagePanelUsingChildFlowButton(s);
                    };
                    Button addNewButton = new Button
                    {
                        AutoSize = true,
                        Text = "Add",
                    };
                    addNewButton.Click += (object s, EventArgs re) =>
                    {
                        Range selectedRange = _owner.RichTextBox.GetSelectedRange();
                        ranges.Add(selectedRange);
                        MessagePanel.Show(_owner.RichTextBox, $"\"{_owner.RichTextBox.SelectedText}\" will be added once you click OK.");
                        _owner.RichTextBox.Select(selectedRange.Start, 0);
                    };
                    MessagePanel.Show(_owner.RichTextBox, "Edit the source selection for this node or add new nodes.", durationInSeconds: 0, flowControls: new Control[] { okButton, addNewButton, cancelButton });
                };
                return rangesButton;
            }

            private static void RemoveMessagePanelUsingChildFlowButton(object s)
            {
                if (!(s is Control control) || !(control?.Parent?.Parent is Panel panel)) throw new InvalidOperationException();
                panel.Parent.Controls.Remove(panel);
            }

            public void ChangeStartingNumber(string id)
            {
                var node = GetValidNode(id);
                var result = InputDialog<int>.ShowDialog("Numbered List", "Start At", Math.Max(1, node.StartAtNumber));
                if (result != null)
                {
                    node.StartAtNumber = Math.Max(1, result.Item1);
                    _owner.UpdateIndex();
                }
            }

            public void ToggleChildrenType(string id)
            {
                var node = GetValidNode(id);
                node.NumberedChildren = !node.NumberedChildren;
                _owner.UpdateIndex();
            }

            public void Move(string id, MoveActions actions)
            {
                var node = GetValidNode(id);
                if (node.Parent == null) return;
                var siblingNodes = node.Parent.Children;
                int currentIndex = siblingNodes.IndexOf(node);

                int newIndex = GetNewIndex(currentIndex, siblingNodes.Count, actions);

                if (newIndex != currentIndex)
                {
                    siblingNodes.RemoveAt(currentIndex);
                    siblingNodes.Insert(newIndex, node);
                    _owner.UpdateIndex();
                }
            }

            private int GetNewIndex(int currentIndex, int siblingsCount, MoveActions actions)
            {
                switch (actions)
                {
                    case MoveActions.Top:
                        return 0;
                    case MoveActions.Up:
                        return Math.Max(currentIndex - 1, 0);
                    case MoveActions.Down:
                        return Math.Min(currentIndex + 1, siblingsCount - 1);
                    case MoveActions.Bottom:
                        return siblingsCount - 1;
                    default:
                        return currentIndex;
                }
            }

            public void Export(IOutlineExporter exporter)
            {
                if (exporter == null) throw new ArgumentNullException(nameof(exporter));
                exporter.Export(_owner); 
            }
        }
    }
}
