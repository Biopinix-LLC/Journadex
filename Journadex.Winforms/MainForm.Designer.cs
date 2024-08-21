namespace Journadex.Winforms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainSplit = new System.Windows.Forms.SplitContainer();
            this.monthCalendar = new System.Windows.Forms.MonthCalendar();
            this.tabs = new System.Windows.Forms.TabControl();
            this.documentTab = new System.Windows.Forms.TabPage();
            this.document = new System.Windows.Forms.RichTextBox();
            this.indexTab = new System.Windows.Forms.TabPage();
            this.index = new System.Windows.Forms.WebBrowser();
            this.outlineTab = new System.Windows.Forms.TabPage();
            this.outline = new System.Windows.Forms.WebBrowser();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).BeginInit();
            this.mainSplit.Panel1.SuspendLayout();
            this.mainSplit.Panel2.SuspendLayout();
            this.mainSplit.SuspendLayout();
            this.tabs.SuspendLayout();
            this.documentTab.SuspendLayout();
            this.indexTab.SuspendLayout();
            this.outlineTab.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainSplit
            // 
            this.mainSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.mainSplit.Location = new System.Drawing.Point(0, 32);
            this.mainSplit.Name = "mainSplit";
            this.mainSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplit.Panel1
            // 
            this.mainSplit.Panel1.Controls.Add(this.monthCalendar);
            // 
            // mainSplit.Panel2
            // 
            this.mainSplit.Panel2.Controls.Add(this.tabs);
            this.mainSplit.Size = new System.Drawing.Size(800, 418);
            this.mainSplit.SplitterDistance = 213;
            this.mainSplit.TabIndex = 0;
            // 
            // monthCalendar
            // 
            this.monthCalendar.CalendarDimensions = new System.Drawing.Size(3, 1);
            this.monthCalendar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monthCalendar.Location = new System.Drawing.Point(0, 0);
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.TabIndex = 0;
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.documentTab);
            this.tabs.Controls.Add(this.indexTab);
            this.tabs.Controls.Add(this.outlineTab);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(800, 201);
            this.tabs.TabIndex = 0;
            // 
            // documentTab
            // 
            this.documentTab.Controls.Add(this.document);
            this.documentTab.Location = new System.Drawing.Point(4, 25);
            this.documentTab.Name = "documentTab";
            this.documentTab.Padding = new System.Windows.Forms.Padding(3);
            this.documentTab.Size = new System.Drawing.Size(792, 172);
            this.documentTab.TabIndex = 0;
            this.documentTab.Text = "Document";
            this.documentTab.UseVisualStyleBackColor = true;
            // 
            // document
            // 
            this.document.Dock = System.Windows.Forms.DockStyle.Fill;
            this.document.Location = new System.Drawing.Point(3, 3);
            this.document.Name = "document";
            this.document.Size = new System.Drawing.Size(786, 166);
            this.document.TabIndex = 0;
            this.document.Text = "";
            this.document.TextChanged += new System.EventHandler(this.document_TextChanged);
            // 
            // indexTab
            // 
            this.indexTab.Controls.Add(this.index);
            this.indexTab.Location = new System.Drawing.Point(4, 25);
            this.indexTab.Name = "indexTab";
            this.indexTab.Padding = new System.Windows.Forms.Padding(3);
            this.indexTab.Size = new System.Drawing.Size(792, 172);
            this.indexTab.TabIndex = 1;
            this.indexTab.Text = "Index";
            this.indexTab.UseVisualStyleBackColor = true;
            // 
            // index
            // 
            this.index.Dock = System.Windows.Forms.DockStyle.Fill;
            this.index.Location = new System.Drawing.Point(3, 3);
            this.index.Name = "index";
            this.index.Size = new System.Drawing.Size(786, 166);
            this.index.TabIndex = 0;
            // 
            // outlineTab
            // 
            this.outlineTab.Controls.Add(this.outline);
            this.outlineTab.Location = new System.Drawing.Point(4, 25);
            this.outlineTab.Name = "outlineTab";
            this.outlineTab.Size = new System.Drawing.Size(792, 172);
            this.outlineTab.TabIndex = 2;
            this.outlineTab.Text = "Outline";
            this.outlineTab.UseVisualStyleBackColor = true;
            // 
            // outline
            // 
            this.outline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outline.Location = new System.Drawing.Point(0, 0);
            this.outline.Name = "outline";
            this.outline.Size = new System.Drawing.Size(792, 172);
            this.outline.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.findToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 32);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 28);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.AutoSize = false;
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.Size = new System.Drawing.Size(400, 28);
            this.findToolStripMenuItem.Text = "Find";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.mainSplit);
            this.Controls.Add(this.menuStrip1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.mainSplit.Panel1.ResumeLayout(false);
            this.mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).EndInit();
            this.mainSplit.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.documentTab.ResumeLayout(false);
            this.indexTab.ResumeLayout(false);
            this.outlineTab.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer mainSplit;
        private System.Windows.Forms.MonthCalendar monthCalendar;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage documentTab;
        private System.Windows.Forms.TabPage indexTab;
        private System.Windows.Forms.TabPage outlineTab;
        private System.Windows.Forms.RichTextBox document;
        private System.Windows.Forms.WebBrowser index;
        private System.Windows.Forms.WebBrowser outline;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox findToolStripMenuItem;
    }
}

