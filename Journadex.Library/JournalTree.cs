using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journadex.Library
{
    public class JournalTree
    {



       
        public enum NodeType { None, Root, Entry, Date, Event, Place, Person }
        public class JournalNode
        {
            private NodeType _type;

            public Range Range { get; }
            public JournalNode Parent { get; }
            public List<JournalNode> Children { get; }
            public NodeType Type
            {
                get => _type; set
                {
                    switch (_type)
                    {
                        case NodeType.Root:
                        case NodeType.Entry:
                            throw new InvalidOperationException("Cannot change type when Root or Entry.");
                        default:
                            break;
                    }
                    _type = value;
                }
            }
            public DateTime? DateTime { get; }

            public JournalNode(Range range, JournalNode parent, NodeType type, DateTime? dateTime = null)
            {
                Range = range;
                Parent = parent;
                Children = new List<JournalNode>();
                Type = type;
                DateTime = dateTime;
            }
            public void Accept(JournalNodeVisitor visitor)
            {
                visitor.Visit(this);

                // Visit all the children of this node
                foreach (JournalNode child in Children)
                {
                    child.Accept(visitor);
                }
            }
        }

        public JournalNode Root { get; }
        public string Text { get; }
        public string NewLine { get; }

        public JournalTree(string journalText)
        {
            Text = journalText;
            NewLine = journalText.Contains(Environment.NewLine) ? Environment.NewLine : "\n";

            // create root node
            Root = new JournalNode(new Range(0, journalText.Length - 1), null, NodeType.Root);
            
            // split journal text into individual events
            var events = journalText.SplitByDateLine(NewLine);

            // create child nodes for each event and link them to the root node
            foreach (var e in events)
            {
                JournalNode child = new JournalNode(e.Value, Root, NodeType.Entry, e.Key);
                Root.Children.Add(child);
                // Add children to the child for relative dates parsed from the child's text
                if (child.DateTime != null && Text.TryGetDatesFromText(child.Range, child.DateTime.Value, out List<DateTime> dates, out List<Range> ranges))
                {
                    for (int i = 0; i < dates.Count; i++)
                    {
                        JournalNode subChild = new JournalNode(ranges[i], child, NodeType.Event, dates[i]);
                        child.Children.Add(subChild);
                    }

                }

            }
        }

        public void Accept(JournalNodeVisitor visitor)
        {
            if (Root != null)
            {
                Root.Accept(visitor);
            }
        }

        

        // TODO write unit tests
        // TODO load text into tree 
        // TODO build hyperlinked index document 
        // TODO show date as tooltip when hovering over date
        // TODO separate node for date?
        // TODO populate calander with dates
        // TODO clicking on date in calendar navigates to place in text
        // TODO allow adding new node based on current selection or position
        // TODO allow removing nodes from index
        // TODO adding new or removing node should update index
        // TODO find 
        // TODO next/previous 
        // TODO save 
        // TODO updating document updates outline and index


    }

}
