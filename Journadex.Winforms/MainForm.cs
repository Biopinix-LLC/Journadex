using Journadex.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Journadex.Winforms
{
    public partial class MainForm : Form
    {
        private JournalTree _tree = null;
        public MainForm()
        {
            InitializeComponent();

        }

        private void document_TextChanged(object sender, EventArgs e)
        {
            if (document.Text.Length > 0)
            {
                _tree = new JournalTree(document.Text);
                var visitor = new HtmlJournalNodeVisitor(_tree, isIndex: true);                
                _tree.Accept(visitor);
                index.DocumentText = visitor.ToString();
                document.ReadOnly= true;
            }
        }
    }
}
