using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Journadex.Library.JournalTree;

namespace Journadex.Library
{

    public class RtfJournalNodeVisitor : JournalNodeVisitor
    {
        private StringBuilder _rtf = new StringBuilder();

        public RtfJournalNodeVisitor(JournalTree tree, bool isIndex) : base(tree, isIndex) 
        {
            _rtf.Append(@"{\rtf1\ansi\deff0{\fonttbl{\f0 Times New Roman;}}\fs24");
            _rtf.Append(@"{\listlevel\levelnfc0\leveljcn0\levelstartat1\levelfollow0\leveltext\'01\u8226 ;}{\levelnumbers;}");
        }

        public override void Visit(JournalNode node)
        {
            bool asHyperlink = false;
            // Calculate the indent level of the current node
            int indentLevel = GetIndentLevel(node);

            var nodeText = GetNodeText(node);
            // Create the RTF string for the current node
            if (asHyperlink)
            {
                _rtf.Append(@"{\li").Append(new string('\t', indentLevel)).Append(@"\field{\*\fldinst{HYPERLINK ").Append(nodeText).Append(@"}}{\fldrslt{").Append(nodeText).Append("}}}");
            }
            else
            {
                _rtf.Append(@"{\li").Append(new string('\t', indentLevel)).Append(nodeText).Append("}");
            }
        }

        public override string ToString()
        {
            _rtf.Append("}");
            return _rtf.ToString();
        }

    }



}
