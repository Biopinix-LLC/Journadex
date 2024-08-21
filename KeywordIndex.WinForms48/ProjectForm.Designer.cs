namespace KeywordIndex.WinForms48
{
    partial class ProjectForm
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
            this.components = new System.ComponentModel.Container();
            this.calendarDocumentsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.monthCalendar = new System.Windows.Forms.MonthCalendar();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.journalIndexOutlineSplitContainer = new System.Windows.Forms.SplitContainer();
            this.journalIndexSplitContainer = new System.Windows.Forms.SplitContainer();
            this.journalRtf = new System.Windows.Forms.RichTextBox();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.removeFromIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.aliasSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.aliasLabel = new System.Windows.Forms.ToolStripMenuItem();
            this.aliasOf = new System.Windows.Forms.ToolStripComboBox();
            this.tagSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tagsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.addTag = new System.Windows.Forms.ToolStripMenuItem();
            this.tagsSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.cleanHighlights = new System.Windows.Forms.ToolStripMenuItem();
            this.copy = new System.Windows.Forms.ToolStripMenuItem();
            this.copyWithDateReferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indexTabs = new System.Windows.Forms.TabControl();
            this.keywordTab = new System.Windows.Forms.TabPage();
            this.keywordIndexBrowser = new System.Windows.Forms.WebBrowser();
            this.dateTab = new System.Windows.Forms.TabPage();
            this.dateIndex = new System.Windows.Forms.WebBrowser();
            this.outlineBrowser = new System.Windows.Forms.WebBrowser();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.calendarDocumentsSplitContainer)).BeginInit();
            this.calendarDocumentsSplitContainer.Panel1.SuspendLayout();
            this.calendarDocumentsSplitContainer.Panel2.SuspendLayout();
            this.calendarDocumentsSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.journalIndexOutlineSplitContainer)).BeginInit();
            this.journalIndexOutlineSplitContainer.Panel1.SuspendLayout();
            this.journalIndexOutlineSplitContainer.Panel2.SuspendLayout();
            this.journalIndexOutlineSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.journalIndexSplitContainer)).BeginInit();
            this.journalIndexSplitContainer.Panel1.SuspendLayout();
            this.journalIndexSplitContainer.Panel2.SuspendLayout();
            this.journalIndexSplitContainer.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.indexTabs.SuspendLayout();
            this.keywordTab.SuspendLayout();
            this.dateTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // calendarDocumentsSplitContainer
            // 
            this.calendarDocumentsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calendarDocumentsSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.calendarDocumentsSplitContainer.Name = "calendarDocumentsSplitContainer";
            this.calendarDocumentsSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // calendarDocumentsSplitContainer.Panel1
            // 
            this.calendarDocumentsSplitContainer.Panel1.Controls.Add(this.monthCalendar);
            this.calendarDocumentsSplitContainer.Panel1.Controls.Add(this.toolStrip);
            // 
            // calendarDocumentsSplitContainer.Panel2
            // 
            this.calendarDocumentsSplitContainer.Panel2.Controls.Add(this.journalIndexOutlineSplitContainer);
            this.calendarDocumentsSplitContainer.Size = new System.Drawing.Size(800, 450);
            this.calendarDocumentsSplitContainer.SplitterDistance = 155;
            this.calendarDocumentsSplitContainer.TabIndex = 2;
            // 
            // monthCalendar
            // 
            this.monthCalendar.CalendarDimensions = new System.Drawing.Size(3, 1);
            this.monthCalendar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monthCalendar.Location = new System.Drawing.Point(0, 25);
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.TabIndex = 0;
            // 
            // toolStrip
            // 
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(800, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // journalIndexOutlineSplitContainer
            // 
            this.journalIndexOutlineSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.journalIndexOutlineSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.journalIndexOutlineSplitContainer.Name = "journalIndexOutlineSplitContainer";
            // 
            // journalIndexOutlineSplitContainer.Panel1
            // 
            this.journalIndexOutlineSplitContainer.Panel1.Controls.Add(this.journalIndexSplitContainer);
            // 
            // journalIndexOutlineSplitContainer.Panel2
            // 
            this.journalIndexOutlineSplitContainer.Panel2.Controls.Add(this.outlineBrowser);
            this.journalIndexOutlineSplitContainer.Size = new System.Drawing.Size(800, 291);
            this.journalIndexOutlineSplitContainer.SplitterDistance = 613;
            this.journalIndexOutlineSplitContainer.TabIndex = 0;
            // 
            // journalIndexSplitContainer
            // 
            this.journalIndexSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.journalIndexSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.journalIndexSplitContainer.Name = "journalIndexSplitContainer";
            // 
            // journalIndexSplitContainer.Panel1
            // 
            this.journalIndexSplitContainer.Panel1.Controls.Add(this.journalRtf);
            // 
            // journalIndexSplitContainer.Panel2
            // 
            this.journalIndexSplitContainer.Panel2.Controls.Add(this.indexTabs);
            this.journalIndexSplitContainer.Size = new System.Drawing.Size(613, 291);
            this.journalIndexSplitContainer.SplitterDistance = 408;
            this.journalIndexSplitContainer.TabIndex = 2;
            // 
            // journalRtf
            // 
            this.journalRtf.ContextMenuStrip = this.contextMenu;
            this.journalRtf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.journalRtf.Font = new System.Drawing.Font("Garamond", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.journalRtf.HideSelection = false;
            this.journalRtf.Location = new System.Drawing.Point(0, 0);
            this.journalRtf.Name = "journalRtf";
            this.journalRtf.ReadOnly = true;
            this.journalRtf.Size = new System.Drawing.Size(408, 291);
            this.journalRtf.TabIndex = 0;
            this.journalRtf.Text = "";
            // 
            // contextMenu
            // 
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToIndex,
            this.removeFromIndex,
            this.aliasSeparator,
            this.aliasLabel,
            this.tagSeparator,
            this.tagsMenu,
            this.toolStripSeparator1,
            this.cleanHighlights,
            this.toolStripSeparator2,
            this.copy,
            this.copyWithDateReferenceToolStripMenuItem,
            this.toolStripSeparator3,
            this.findToolStripMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(251, 254);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenu_Opening);
            // 
            // addToIndex
            // 
            this.addToIndex.Name = "addToIndex";
            this.addToIndex.Size = new System.Drawing.Size(250, 24);
            this.addToIndex.Text = "Add to Index";
            this.addToIndex.Click += new System.EventHandler(this.AddToIndex_Click);
            // 
            // removeFromIndex
            // 
            this.removeFromIndex.Name = "removeFromIndex";
            this.removeFromIndex.Size = new System.Drawing.Size(250, 24);
            this.removeFromIndex.Text = "Remove from Index";
            this.removeFromIndex.Click += new System.EventHandler(this.RemoveFromIndex_Click);
            // 
            // aliasSeparator
            // 
            this.aliasSeparator.Name = "aliasSeparator";
            this.aliasSeparator.Size = new System.Drawing.Size(247, 6);
            // 
            // aliasLabel
            // 
            this.aliasLabel.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aliasOf});
            this.aliasLabel.Name = "aliasLabel";
            this.aliasLabel.Size = new System.Drawing.Size(250, 24);
            this.aliasLabel.Text = "Alias of";
            // 
            // aliasOf
            // 
            this.aliasOf.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.aliasOf.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.aliasOf.Name = "aliasOf";
            this.aliasOf.Size = new System.Drawing.Size(210, 28);
            this.aliasOf.Sorted = true;
            this.aliasOf.SelectedIndexChanged += new System.EventHandler(this.AliasOf_SelectedIndexChanged);
            // 
            // tagSeparator
            // 
            this.tagSeparator.Name = "tagSeparator";
            this.tagSeparator.Size = new System.Drawing.Size(247, 6);
            // 
            // tagsMenu
            // 
            this.tagsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addTag,
            this.tagsSeparator});
            this.tagsMenu.Name = "tagsMenu";
            this.tagsMenu.Size = new System.Drawing.Size(250, 24);
            this.tagsMenu.Text = "Tags";
            // 
            // addTag
            // 
            this.addTag.Name = "addTag";
            this.addTag.Size = new System.Drawing.Size(156, 26);
            this.addTag.Text = "Add Tag...";
            this.addTag.Click += new System.EventHandler(this.AddTag_Click);
            // 
            // tagsSeparator
            // 
            this.tagsSeparator.Name = "tagsSeparator";
            this.tagsSeparator.Size = new System.Drawing.Size(153, 6);
            this.tagsSeparator.Visible = false;
            // 
            // cleanHighlights
            // 
            this.cleanHighlights.Name = "cleanHighlights";
            this.cleanHighlights.Size = new System.Drawing.Size(250, 24);
            this.cleanHighlights.Text = "Clean Highlights";
            this.cleanHighlights.Click += new System.EventHandler(this.CleanHighlights_Click);
            // 
            // copy
            // 
            this.copy.Name = "copy";
            this.copy.Size = new System.Drawing.Size(250, 24);
            this.copy.Text = "Copy";
            this.copy.Click += new System.EventHandler(this.Copy_Click);
            // 
            // copyWithDateReferenceToolStripMenuItem
            // 
            this.copyWithDateReferenceToolStripMenuItem.Name = "copyWithDateReferenceToolStripMenuItem";
            this.copyWithDateReferenceToolStripMenuItem.Size = new System.Drawing.Size(250, 24);
            this.copyWithDateReferenceToolStripMenuItem.Text = "Copy with Date Reference";
            this.copyWithDateReferenceToolStripMenuItem.Click += new System.EventHandler(this.CopyWithDateReferenceToolStripMenuItem_Click);
            // 
            // indexTabs
            // 
            this.indexTabs.Controls.Add(this.keywordTab);
            this.indexTabs.Controls.Add(this.dateTab);
            this.indexTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indexTabs.Location = new System.Drawing.Point(0, 0);
            this.indexTabs.Name = "indexTabs";
            this.indexTabs.SelectedIndex = 0;
            this.indexTabs.Size = new System.Drawing.Size(201, 291);
            this.indexTabs.TabIndex = 0;
            // 
            // keywordTab
            // 
            this.keywordTab.Controls.Add(this.keywordIndexBrowser);
            this.keywordTab.Location = new System.Drawing.Point(4, 25);
            this.keywordTab.Name = "keywordTab";
            this.keywordTab.Padding = new System.Windows.Forms.Padding(3);
            this.keywordTab.Size = new System.Drawing.Size(193, 262);
            this.keywordTab.TabIndex = 0;
            this.keywordTab.Text = "Keyword Index";
            this.keywordTab.UseVisualStyleBackColor = true;
            // 
            // keywordIndexBrowser
            // 
            this.keywordIndexBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keywordIndexBrowser.Location = new System.Drawing.Point(3, 3);
            this.keywordIndexBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.keywordIndexBrowser.Name = "keywordIndexBrowser";
            this.keywordIndexBrowser.Size = new System.Drawing.Size(187, 256);
            this.keywordIndexBrowser.TabIndex = 1;
            // 
            // dateTab
            // 
            this.dateTab.Controls.Add(this.dateIndex);
            this.dateTab.Location = new System.Drawing.Point(4, 25);
            this.dateTab.Name = "dateTab";
            this.dateTab.Padding = new System.Windows.Forms.Padding(3);
            this.dateTab.Size = new System.Drawing.Size(193, 262);
            this.dateTab.TabIndex = 1;
            this.dateTab.Text = "Date Index";
            this.dateTab.UseVisualStyleBackColor = true;
            // 
            // dateIndex
            // 
            this.dateIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dateIndex.Location = new System.Drawing.Point(3, 3);
            this.dateIndex.MinimumSize = new System.Drawing.Size(20, 20);
            this.dateIndex.Name = "dateIndex";
            this.dateIndex.Size = new System.Drawing.Size(187, 256);
            this.dateIndex.TabIndex = 2;
            // 
            // outlineBrowser
            // 
            this.outlineBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outlineBrowser.Location = new System.Drawing.Point(0, 0);
            this.outlineBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.outlineBrowser.Name = "outlineBrowser";
            this.outlineBrowser.Size = new System.Drawing.Size(183, 291);
            this.outlineBrowser.TabIndex = 2;
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.Size = new System.Drawing.Size(250, 24);
            this.findToolStripMenuItem.Text = "Find...";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.FindToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(247, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(247, 6);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(247, 6);
            // 
            // ProjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.calendarDocumentsSplitContainer);
            this.Name = "ProjectForm";
            this.Text = "ProjectForm";
            this.calendarDocumentsSplitContainer.Panel1.ResumeLayout(false);
            this.calendarDocumentsSplitContainer.Panel1.PerformLayout();
            this.calendarDocumentsSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.calendarDocumentsSplitContainer)).EndInit();
            this.calendarDocumentsSplitContainer.ResumeLayout(false);
            this.journalIndexOutlineSplitContainer.Panel1.ResumeLayout(false);
            this.journalIndexOutlineSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.journalIndexOutlineSplitContainer)).EndInit();
            this.journalIndexOutlineSplitContainer.ResumeLayout(false);
            this.journalIndexSplitContainer.Panel1.ResumeLayout(false);
            this.journalIndexSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.journalIndexSplitContainer)).EndInit();
            this.journalIndexSplitContainer.ResumeLayout(false);
            this.contextMenu.ResumeLayout(false);
            this.indexTabs.ResumeLayout(false);
            this.keywordTab.ResumeLayout(false);
            this.dateTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer calendarDocumentsSplitContainer;
        private System.Windows.Forms.MonthCalendar monthCalendar;
        private System.Windows.Forms.ToolStrip toolStrip;
        internal System.Windows.Forms.SplitContainer journalIndexOutlineSplitContainer;
        internal System.Windows.Forms.SplitContainer journalIndexSplitContainer;
        private System.Windows.Forms.RichTextBox journalRtf;
        private System.Windows.Forms.TabControl indexTabs;
        private System.Windows.Forms.TabPage keywordTab;
        private System.Windows.Forms.WebBrowser keywordIndexBrowser;
        private System.Windows.Forms.TabPage dateTab;
        private System.Windows.Forms.WebBrowser dateIndex;
        private System.Windows.Forms.WebBrowser outlineBrowser;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem addToIndex;
        private System.Windows.Forms.ToolStripMenuItem removeFromIndex;
        private System.Windows.Forms.ToolStripSeparator aliasSeparator;
        private System.Windows.Forms.ToolStripMenuItem aliasLabel;
        private System.Windows.Forms.ToolStripComboBox aliasOf;
        private System.Windows.Forms.ToolStripSeparator tagSeparator;
        private System.Windows.Forms.ToolStripMenuItem tagsMenu;
        private System.Windows.Forms.ToolStripMenuItem addTag;
        private System.Windows.Forms.ToolStripSeparator tagsSeparator;
        private System.Windows.Forms.ToolStripMenuItem cleanHighlights;
        private System.Windows.Forms.ToolStripMenuItem copy;
        private System.Windows.Forms.ToolStripMenuItem copyWithDateReferenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
    }
}