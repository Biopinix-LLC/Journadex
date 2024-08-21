using Journadex.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace KeywordIndex.WinForms48
{
    public partial class Outline
    {
        internal override Dictionary<string, List<Range>> GetIndex()
        {
            throw new NotImplementedException();
        }

        
        public class Node 
        {

            public string Id { get; } = Guid.NewGuid().ToString();
            /// <summary>
            /// The source of the text that is visible in the outline (i.e. the sentence)
            /// </summary>
            public Range VisibleSource { get; set; }
            /// <summary>
            /// The source of the text that is used by the index to link to the outline node (i.e. the keyword or date range)
            /// </summary>
            public Range IndexSource { get; set; }
            public string Notes { get; set; }
            public Node Parent { get; private set; }
            public DateTime? Date { get; private set; }
            public List<Node> Children { get; private set; } = new List<Node>();
            public bool NumberedChildren { get; set; }
            public int StartAtNumber { get; set; } = 1;
            public bool IsSourceDateIndex { get; set; }
            public bool IsSourceUserEdited { get; set; }
            [JsonIgnore]
            public Range Source => VisibleSource ?? IndexSource;

            public Node(Node parent = null, Range visibleSource = null, string notes = null, DateTime? date = null, bool isSourceDateIndex = false, Range indexSource = null)
            {

                VisibleSource = visibleSource;
                Notes = notes;
                Parent = parent;
                Date = date;
                IsSourceDateIndex = isSourceDateIndex;
                IndexSource = indexSource;
            }

            internal void Move(Node fromParent, Node toParent)
            {
                if (fromParent == null) throw new ArgumentNullException(nameof(fromParent));
                if (toParent == null) throw new ArgumentNullException(nameof(toParent));

                fromParent.Children.Remove(this);
                Parent = toParent;
                Parent.Children.Add(this);
            }

            /// <summary>
            /// ChatGPT recommended and generated this method.
            /// </summary>
            /// <returns></returns>
            public Node Clone()
            {
                Node clone = new Node();
                //clone.Id = Id;
                clone.VisibleSource = VisibleSource?.Clone();
                clone.IndexSource = IndexSource?.Clone();
                clone.Notes = Notes;
                clone.Parent = null; // set to null by default
                clone.Date = Date;
                clone.NumberedChildren = NumberedChildren;
                clone.StartAtNumber = StartAtNumber;
                clone.IsSourceDateIndex = IsSourceDateIndex;
                clone.IsSourceUserEdited = IsSourceUserEdited;
                clone.Children = new List<Node>();

                foreach (Node child in Children)
                {
                    Node childClone = child.Clone();
                    childClone.SetParent(clone);
                    clone.Children.Add(childClone);
                }

                return clone;
            }

            internal void SetParent(Node node)
            {
                Parent = node;
                foreach (Node child in Children)
                {
                    child.SetParent(this);
                }
            }

            public Node DeepCopy() => Clone();

            internal void AddSibling(Node sibling, bool pasteAbove, Node root)
            {
                if (sibling == null)
                {
                    throw new ArgumentNullException(nameof(sibling));
                }
                var parent = Parent ?? root;
                if (parent == null)
                {
                    throw new InvalidOperationException("Cannot paste as sibling when node has no parent.");
                }

                var siblings = parent.Children;
                var index = siblings.IndexOf(this);

                if (pasteAbove)
                {
                    siblings.Insert(index, sibling);
                }
                else if (index == siblings.Count - 1)
                {
                    siblings.Add(sibling);
                }
                else
                {
                    siblings.Insert(index + 1, sibling);
                }
            }
        }
    }
}
