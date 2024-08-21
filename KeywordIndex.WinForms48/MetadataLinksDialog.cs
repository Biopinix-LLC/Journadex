using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeywordIndex.WinForms48
{
    public partial class MetadataLinksDialog : Form
    {
        public MetadataLinksDialog(string keyword, List<string> metadata)
        {
            InitializeComponent();
            Size = new Size(300, 250);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Metadata = metadata;
            Keyword = keyword;
            MetadataLinksControl control = new MetadataLinksControl(keyword, metadata)
            {
                Dock = DockStyle.Fill
            };
            Text = keyword;
            Controls.Add(control);
        }

        public List<string> Metadata { get; internal set; }
        public string Keyword { get; internal set; }
    }
}
