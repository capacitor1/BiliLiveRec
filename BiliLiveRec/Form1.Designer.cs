namespace BiliLiveRec
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            UI_Rooms = new ListView();
            columnHeader1 = new ColumnHeader();
            contextMenuStrip1 = new ContextMenuStrip(components);
            StartRToolStripMenuItem = new ToolStripMenuItem();
            StopRToolStripMenuItem = new ToolStripMenuItem();
            StopR1ToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            OpenURL = new ToolStripMenuItem();
            CopyRIDToolStripMenuItem = new ToolStripMenuItem();
            IconImg = new ImageList(components);
            menuStrip1 = new MenuStrip();
            RoomsToolStripMenuItem = new ToolStripMenuItem();
            AddToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            StartRecToolStripMenuItem = new ToolStripMenuItem();
            StopRecToolStripMenuItem = new ToolStripMenuItem();
            StopRec1ToolStripMenuItem = new ToolStripMenuItem();
            LogToolStripMenuItem = new ToolStripMenuItem();
            ClearToolStripMenuItem = new ToolStripMenuItem();
            ExportToolStripMenuItem = new ToolStripMenuItem();
            设置ToolStripMenuItem = new ToolStripMenuItem();
            RecPathToolStripMenuItem = new ToolStripMenuItem();
            AutoScrollToolStripMenuItem = new ToolStripMenuItem();
            UI_LogView = new RichTextBox();
            label1 = new Label();
            UI_StatView = new Label();
            ForceExit = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // UI_Rooms
            // 
            UI_Rooms.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            UI_Rooms.Columns.AddRange(new ColumnHeader[] { columnHeader1 });
            UI_Rooms.ContextMenuStrip = contextMenuStrip1;
            UI_Rooms.FullRowSelect = true;
            UI_Rooms.LargeImageList = IconImg;
            UI_Rooms.Location = new Point(0, 28);
            UI_Rooms.Name = "UI_Rooms";
            UI_Rooms.Size = new Size(704, 107);
            UI_Rooms.SmallImageList = IconImg;
            UI_Rooms.TabIndex = 0;
            UI_Rooms.UseCompatibleStateImageBehavior = false;
            UI_Rooms.KeyDown += UI_Rooms_KeyDown;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "RoomID";
            columnHeader1.Width = 100;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { StartRToolStripMenuItem, StopRToolStripMenuItem, StopR1ToolStripMenuItem, ForceExit, toolStripSeparator2, OpenURL, CopyRIDToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(197, 164);
            // 
            // StartRToolStripMenuItem
            // 
            StartRToolStripMenuItem.Name = "StartRToolStripMenuItem";
            StartRToolStripMenuItem.Size = new Size(196, 22);
            StartRToolStripMenuItem.Text = "启动录制";
            StartRToolStripMenuItem.Click += StartRToolStripMenuItem_Click;
            // 
            // StopRToolStripMenuItem
            // 
            StopRToolStripMenuItem.Name = "StopRToolStripMenuItem";
            StopRToolStripMenuItem.Size = new Size(196, 22);
            StopRToolStripMenuItem.Text = "停止录制";
            StopRToolStripMenuItem.Click += StopRToolStripMenuItem_Click;
            // 
            // StopR1ToolStripMenuItem
            // 
            StopR1ToolStripMenuItem.Name = "StopR1ToolStripMenuItem";
            StopR1ToolStripMenuItem.Size = new Size(196, 22);
            StopR1ToolStripMenuItem.Text = "停止录制（关闭线程）";
            StopR1ToolStripMenuItem.Click += StopR1ToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(193, 6);
            // 
            // OpenURL
            // 
            OpenURL.Name = "OpenURL";
            OpenURL.Size = new Size(196, 22);
            OpenURL.Text = "在浏览器中打开";
            OpenURL.Click += OpenURL_Click;
            // 
            // CopyRIDToolStripMenuItem
            // 
            CopyRIDToolStripMenuItem.Name = "CopyRIDToolStripMenuItem";
            CopyRIDToolStripMenuItem.Size = new Size(196, 22);
            CopyRIDToolStripMenuItem.Text = "复制房间ID";
            CopyRIDToolStripMenuItem.Click += CopyRIDToolStripMenuItem_Click;
            // 
            // IconImg
            // 
            IconImg.ColorDepth = ColorDepth.Depth32Bit;
            IconImg.ImageStream = (ImageListStreamer)resources.GetObject("IconImg.ImageStream");
            IconImg.TransparentColor = Color.Transparent;
            IconImg.Images.SetKeyName(0, "0.ico");
            IconImg.Images.SetKeyName(1, "1.ico");
            IconImg.Images.SetKeyName(2, "2.ico");
            IconImg.Images.SetKeyName(3, "3.ico");
            IconImg.Images.SetKeyName(4, "4.ico");
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { RoomsToolStripMenuItem, LogToolStripMenuItem, 设置ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(704, 25);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // RoomsToolStripMenuItem
            // 
            RoomsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { AddToolStripMenuItem, toolStripSeparator1, StartRecToolStripMenuItem, StopRecToolStripMenuItem, StopRec1ToolStripMenuItem });
            RoomsToolStripMenuItem.Name = "RoomsToolStripMenuItem";
            RoomsToolStripMenuItem.Size = new Size(44, 21);
            RoomsToolStripMenuItem.Text = "房间";
            // 
            // AddToolStripMenuItem
            // 
            AddToolStripMenuItem.Name = "AddToolStripMenuItem";
            AddToolStripMenuItem.Size = new Size(196, 22);
            AddToolStripMenuItem.Text = "添加";
            AddToolStripMenuItem.Click += AddToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(193, 6);
            // 
            // StartRecToolStripMenuItem
            // 
            StartRecToolStripMenuItem.Name = "StartRecToolStripMenuItem";
            StartRecToolStripMenuItem.Size = new Size(196, 22);
            StartRecToolStripMenuItem.Text = "启动录制";
            StartRecToolStripMenuItem.Click += StartRecToolStripMenuItem_Click;
            // 
            // StopRecToolStripMenuItem
            // 
            StopRecToolStripMenuItem.Name = "StopRecToolStripMenuItem";
            StopRecToolStripMenuItem.Size = new Size(196, 22);
            StopRecToolStripMenuItem.Text = "停止录制";
            StopRecToolStripMenuItem.Click += StopRecToolStripMenuItem_Click;
            // 
            // StopRec1ToolStripMenuItem
            // 
            StopRec1ToolStripMenuItem.Name = "StopRec1ToolStripMenuItem";
            StopRec1ToolStripMenuItem.Size = new Size(196, 22);
            StopRec1ToolStripMenuItem.Text = "停止录制（关闭线程）";
            StopRec1ToolStripMenuItem.Click += StopRec1ToolStripMenuItem_Click;
            // 
            // LogToolStripMenuItem
            // 
            LogToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { ClearToolStripMenuItem, ExportToolStripMenuItem });
            LogToolStripMenuItem.Name = "LogToolStripMenuItem";
            LogToolStripMenuItem.Size = new Size(44, 21);
            LogToolStripMenuItem.Text = "日志";
            // 
            // ClearToolStripMenuItem
            // 
            ClearToolStripMenuItem.Name = "ClearToolStripMenuItem";
            ClearToolStripMenuItem.Size = new Size(124, 22);
            ClearToolStripMenuItem.Text = "清空";
            ClearToolStripMenuItem.Click += ClearToolStripMenuItem_Click;
            // 
            // ExportToolStripMenuItem
            // 
            ExportToolStripMenuItem.Name = "ExportToolStripMenuItem";
            ExportToolStripMenuItem.Size = new Size(124, 22);
            ExportToolStripMenuItem.Text = "导出文本";
            ExportToolStripMenuItem.Click += ExportToolStripMenuItem_Click;
            // 
            // 设置ToolStripMenuItem
            // 
            设置ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { RecPathToolStripMenuItem, AutoScrollToolStripMenuItem });
            设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            设置ToolStripMenuItem.Size = new Size(44, 21);
            设置ToolStripMenuItem.Text = "设置";
            // 
            // RecPathToolStripMenuItem
            // 
            RecPathToolStripMenuItem.Name = "RecPathToolStripMenuItem";
            RecPathToolStripMenuItem.Size = new Size(184, 22);
            RecPathToolStripMenuItem.Text = "更改录制文件夹";
            RecPathToolStripMenuItem.Click += RecPathToolStripMenuItem_Click;
            // 
            // AutoScrollToolStripMenuItem
            // 
            AutoScrollToolStripMenuItem.Checked = true;
            AutoScrollToolStripMenuItem.CheckOnClick = true;
            AutoScrollToolStripMenuItem.CheckState = CheckState.Checked;
            AutoScrollToolStripMenuItem.Name = "AutoScrollToolStripMenuItem";
            AutoScrollToolStripMenuItem.Size = new Size(184, 22);
            AutoScrollToolStripMenuItem.Text = "日志自动滚动到底部";
            // 
            // UI_LogView
            // 
            UI_LogView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            UI_LogView.Location = new Point(0, 158);
            UI_LogView.Name = "UI_LogView";
            UI_LogView.ReadOnly = true;
            UI_LogView.Size = new Size(704, 284);
            UI_LogView.TabIndex = 2;
            UI_LogView.Text = "";
            UI_LogView.TextChanged += UI_LogView_TextChanged;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(12, 138);
            label1.Name = "label1";
            label1.Size = new Size(68, 17);
            label1.TabIndex = 3;
            label1.Text = "统计数据：";
            // 
            // UI_StatView
            // 
            UI_StatView.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            UI_StatView.Location = new Point(142, 138);
            UI_StatView.Name = "UI_StatView";
            UI_StatView.Size = new Size(550, 17);
            UI_StatView.TabIndex = 4;
            UI_StatView.Text = "已启动：0 活动：0 | 已下载： 0（0 字节）";
            UI_StatView.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ForceExit
            // 
            ForceExit.Name = "ForceExit";
            ForceExit.Size = new Size(196, 22);
            ForceExit.Text = "强制停止";
            ForceExit.Click += ForceExit_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(704, 441);
            Controls.Add(UI_StatView);
            Controls.Add(label1);
            Controls.Add(UI_LogView);
            Controls.Add(UI_Rooms);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "BiliLiveRec";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            contextMenuStrip1.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView UI_Rooms;
        private ColumnHeader columnHeader1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem RoomsToolStripMenuItem;
        private ToolStripMenuItem AddToolStripMenuItem;
        private RichTextBox UI_LogView;
        private ToolStripMenuItem LogToolStripMenuItem;
        private ToolStripMenuItem ClearToolStripMenuItem;
        private ToolStripMenuItem ExportToolStripMenuItem;
        private ImageList IconImg;
        private Label label1;
        private ToolStripMenuItem StartRecToolStripMenuItem;
        private ToolStripMenuItem 设置ToolStripMenuItem;
        private ToolStripMenuItem RecPathToolStripMenuItem;
        private ToolStripMenuItem StopRecToolStripMenuItem;
        private ToolStripMenuItem StopRec1ToolStripMenuItem;
        private Label UI_StatView;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem StartRToolStripMenuItem;
        private ToolStripMenuItem StopRToolStripMenuItem;
        private ToolStripMenuItem StopR1ToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem OpenURL;
        private ToolStripMenuItem AutoScrollToolStripMenuItem;
        private ToolStripMenuItem CopyRIDToolStripMenuItem;
        private ToolStripMenuItem ForceExit;
    }
}
