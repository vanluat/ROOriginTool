namespace AssetsUpdate.Features.TableUpdate
{
    partial class TableUpdateCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            groupBox1 = new GroupBox();
            uiTableList = new ListBox();
            uiTableSearch = new TextBox();
            groupBox2 = new GroupBox();
            uiTableView = new ListView();
            contextMenuStrip1 = new ContextMenuStrip(components);
            loadOfficialTableToolStripMenuItem = new ToolStripMenuItem();
            compareOfficialTableToolStripMenuItem = new ToolStripMenuItem();
            saveTableToolStripMenuItem = new ToolStripMenuItem();
            label1 = new Label();
            label2 = new Label();
            uiTableCount = new Label();
            uiRowCount = new Label();
            uictxColumnMenu = new ContextMenuStrip(components);
            setColumnTranslateToolStripMenuItem = new ToolStripMenuItem();
            uiMenuSetSingleIdTransle = new ToolStripMenuItem();
            uiMenuSetArrayIdTranslate = new ToolStripMenuItem();
            uiMenuOverrideColumnIndex = new ToolStripMenuItem();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            uictxColumnMenu.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBox1.Controls.Add(uiTableList);
            groupBox1.Controls.Add(uiTableSearch);
            groupBox1.Location = new Point(3, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(200, 517);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            // 
            // uiTableList
            // 
            uiTableList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            uiTableList.FormattingEnabled = true;
            uiTableList.HorizontalScrollbar = true;
            uiTableList.ItemHeight = 15;
            uiTableList.Location = new Point(6, 49);
            uiTableList.Name = "uiTableList";
            uiTableList.Size = new Size(188, 454);
            uiTableList.TabIndex = 1;
            uiTableList.SelectedValueChanged += uiTableList_SelectedValueChanged;
            // 
            // uiTableSearch
            // 
            uiTableSearch.Location = new Point(6, 13);
            uiTableSearch.Name = "uiTableSearch";
            uiTableSearch.Size = new Size(188, 23);
            uiTableSearch.TabIndex = 0;
            uiTableSearch.TextChanged += uiTableSearch_TextChanged;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(uiTableView);
            groupBox2.Location = new Point(209, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(546, 514);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            // 
            // uiTableView
            // 
            uiTableView.ContextMenuStrip = contextMenuStrip1;
            uiTableView.Dock = DockStyle.Fill;
            uiTableView.FullRowSelect = true;
            uiTableView.GridLines = true;
            uiTableView.Location = new Point(3, 19);
            uiTableView.MultiSelect = false;
            uiTableView.Name = "uiTableView";
            uiTableView.Size = new Size(540, 492);
            uiTableView.TabIndex = 0;
            uiTableView.UseCompatibleStateImageBehavior = false;
            uiTableView.View = View.Details;
            uiTableView.ColumnClick += uiTableView_ColumnClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { loadOfficialTableToolStripMenuItem, compareOfficialTableToolStripMenuItem, saveTableToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(195, 70);
            // 
            // loadOfficialTableToolStripMenuItem
            // 
            loadOfficialTableToolStripMenuItem.Name = "loadOfficialTableToolStripMenuItem";
            loadOfficialTableToolStripMenuItem.Size = new Size(194, 22);
            loadOfficialTableToolStripMenuItem.Text = "Load Official Table";
            loadOfficialTableToolStripMenuItem.Click += loadOfficialTableToolStripMenuItem_Click;
            // 
            // compareOfficialTableToolStripMenuItem
            // 
            compareOfficialTableToolStripMenuItem.Name = "compareOfficialTableToolStripMenuItem";
            compareOfficialTableToolStripMenuItem.Size = new Size(194, 22);
            compareOfficialTableToolStripMenuItem.Text = "Compare Official Table";
            // 
            // saveTableToolStripMenuItem
            // 
            saveTableToolStripMenuItem.Name = "saveTableToolStripMenuItem";
            saveTableToolStripMenuItem.Size = new Size(194, 22);
            saveTableToolStripMenuItem.Text = "Save Table";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(652, 520);
            label1.Name = "label1";
            label1.Size = new Size(46, 15);
            label1.TabIndex = 2;
            label1.Text = "Count: ";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(140, 520);
            label2.Name = "label2";
            label2.Size = new Size(46, 15);
            label2.TabIndex = 3;
            label2.Text = "Count: ";
            // 
            // uiTableCount
            // 
            uiTableCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            uiTableCount.AutoSize = true;
            uiTableCount.Location = new Point(182, 520);
            uiTableCount.Name = "uiTableCount";
            uiTableCount.Size = new Size(13, 15);
            uiTableCount.TabIndex = 4;
            uiTableCount.Text = "0";
            // 
            // uiRowCount
            // 
            uiRowCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            uiRowCount.AutoSize = true;
            uiRowCount.Location = new Point(694, 520);
            uiRowCount.Name = "uiRowCount";
            uiRowCount.Size = new Size(13, 15);
            uiRowCount.TabIndex = 5;
            uiRowCount.Text = "0";
            // 
            // uictxColumnMenu
            // 
            uictxColumnMenu.Items.AddRange(new ToolStripItem[] { setColumnTranslateToolStripMenuItem, uiMenuOverrideColumnIndex });
            uictxColumnMenu.Name = "uictxColumnMenu";
            uictxColumnMenu.Size = new Size(225, 70);
            // 
            // setColumnTranslateToolStripMenuItem
            // 
            setColumnTranslateToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { uiMenuSetSingleIdTransle, uiMenuSetArrayIdTranslate });
            setColumnTranslateToolStripMenuItem.Name = "setColumnTranslateToolStripMenuItem";
            setColumnTranslateToolStripMenuItem.Size = new Size(224, 22);
            setColumnTranslateToolStripMenuItem.Text = "Set This Column To Translate";
            // 
            // uiMenuSetSingleIdTransle
            // 
            uiMenuSetSingleIdTransle.Name = "uiMenuSetSingleIdTransle";
            uiMenuSetSingleIdTransle.Size = new Size(117, 22);
            uiMenuSetSingleIdTransle.Text = "SingleID";
            uiMenuSetSingleIdTransle.Click += uiMenuSetSingleIdTransle_Click;
            // 
            // uiMenuSetArrayIdTranslate
            // 
            uiMenuSetArrayIdTranslate.Name = "uiMenuSetArrayIdTranslate";
            uiMenuSetArrayIdTranslate.Size = new Size(180, 22);
            uiMenuSetArrayIdTranslate.Text = "ArrayID";
            uiMenuSetArrayIdTranslate.Click += uiMenuSetArrayIdTranslate_Click;
            // 
            // uiMenuOverrideColumnIndex
            // 
            uiMenuOverrideColumnIndex.Name = "uiMenuOverrideColumnIndex";
            uiMenuOverrideColumnIndex.Size = new Size(224, 22);
            uiMenuOverrideColumnIndex.Text = "Override Column Index";
            uiMenuOverrideColumnIndex.Click += uiMenuOverrideColumnIndex_Click;
            // 
            // TableUpdateCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(uiRowCount);
            Controls.Add(uiTableCount);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "TableUpdateCtrl";
            Size = new Size(758, 542);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            contextMenuStrip1.ResumeLayout(false);
            uictxColumnMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private TextBox uiTableSearch;
        private ListBox uiTableList;
        private GroupBox groupBox2;
        private ListView uiTableView;
        private Label label1;
        private Label label2;
        private Label uiTableCount;
        private Label uiRowCount;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem loadOfficialTableToolStripMenuItem;
        private ToolStripMenuItem compareOfficialTableToolStripMenuItem;
        private ToolStripMenuItem saveTableToolStripMenuItem;
        private ContextMenuStrip uictxColumnMenu;
        private ToolStripMenuItem setColumnTranslateToolStripMenuItem;
        private ToolStripMenuItem uiMenuSetSingleIdTransle;
        private ToolStripMenuItem uiMenuSetArrayIdTranslate;
        private ToolStripMenuItem uiMenuOverrideColumnIndex;
    }
}
