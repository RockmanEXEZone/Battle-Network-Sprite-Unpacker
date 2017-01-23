namespace MMBN_Sprite_List_Viewer
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openBNSAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.repointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAsBNSAXMLProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCurrentFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportedAnimagedGIFOfAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.theRockmanEXEZoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.romSpritePointersListbox = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.drawScaleUpDown = new System.Windows.Forms.NumericUpDown();
            this.drawScaleLabel = new System.Windows.Forms.Label();
            this.subframeIndexUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.animateCheckbox = new System.Windows.Forms.CheckBox();
            this.romOffsetCheckbox = new System.Windows.Forms.CheckBox();
            this.infoListbox = new System.Windows.Forms.ListBox();
            this.paletteIndexUpDown = new System.Windows.Forms.NumericUpDown();
            this.frameIndexUpDown = new System.Windows.Forms.NumericUpDown();
            this.animationIndexUpDown = new System.Windows.Forms.NumericUpDown();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.spriteanimator = new System.ComponentModel.BackgroundWorker();
            this.animationCountLabel = new System.Windows.Forms.Label();
            this.frameCountLabel = new System.Windows.Forms.Label();
            this.subframeCountLabel = new System.Windows.Forms.Label();
            this.paletteCountLabel = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.drawScaleUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.subframeIndexUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteIndexUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameIndexUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.animationIndexUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.listToolStripMenuItem,
            this.spriteToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(985, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openROMToolStripMenuItem,
            this.openBNSAToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openROMToolStripMenuItem
            // 
            this.openROMToolStripMenuItem.Name = "openROMToolStripMenuItem";
            this.openROMToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openROMToolStripMenuItem.Size = new System.Drawing.Size(221, 26);
            this.openROMToolStripMenuItem.Text = "Open ROM";
            this.openROMToolStripMenuItem.Click += new System.EventHandler(this.openROMToolStripMenuItem_Click);
            // 
            // openBNSAToolStripMenuItem
            // 
            this.openBNSAToolStripMenuItem.Name = "openBNSAToolStripMenuItem";
            this.openBNSAToolStripMenuItem.Size = new System.Drawing.Size(221, 26);
            this.openBNSAToolStripMenuItem.Text = "Open BNSA";
            this.openBNSAToolStripMenuItem.Click += new System.EventHandler(this.openBNSAToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(221, 26);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(221, 26);
            this.saveAsToolStripMenuItem.Text = "SaveAs";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(221, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // listToolStripMenuItem
            // 
            this.listToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchListToolStripMenuItem,
            this.repointToolStripMenuItem,
            this.getCodeToolStripMenuItem});
            this.listToolStripMenuItem.Name = "listToolStripMenuItem";
            this.listToolStripMenuItem.Size = new System.Drawing.Size(43, 24);
            this.listToolStripMenuItem.Text = "List";
            // 
            // searchListToolStripMenuItem
            // 
            this.searchListToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchToolStripMenuItem,
            this.nextToolStripMenuItem,
            this.previousToolStripMenuItem});
            this.searchListToolStripMenuItem.Name = "searchListToolStripMenuItem";
            this.searchListToolStripMenuItem.Size = new System.Drawing.Size(154, 26);
            this.searchListToolStripMenuItem.Text = "Search List";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(177, 26);
            this.searchToolStripMenuItem.Text = "Search";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.searchToolStripMenuItem_Click);
            // 
            // nextToolStripMenuItem
            // 
            this.nextToolStripMenuItem.Name = "nextToolStripMenuItem";
            this.nextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.nextToolStripMenuItem.Size = new System.Drawing.Size(177, 26);
            this.nextToolStripMenuItem.Text = "Next";
            this.nextToolStripMenuItem.Click += new System.EventHandler(this.nextToolStripMenuItem_Click);
            // 
            // previousToolStripMenuItem
            // 
            this.previousToolStripMenuItem.Name = "previousToolStripMenuItem";
            this.previousToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.previousToolStripMenuItem.Size = new System.Drawing.Size(177, 26);
            this.previousToolStripMenuItem.Text = "Previous";
            this.previousToolStripMenuItem.Click += new System.EventHandler(this.previousToolStripMenuItem_Click);
            // 
            // repointToolStripMenuItem
            // 
            this.repointToolStripMenuItem.Name = "repointToolStripMenuItem";
            this.repointToolStripMenuItem.Size = new System.Drawing.Size(154, 26);
            this.repointToolStripMenuItem.Text = "Repoint";
            this.repointToolStripMenuItem.Click += new System.EventHandler(this.repointToolStripMenuItem_Click);
            // 
            // getCodeToolStripMenuItem
            // 
            this.getCodeToolStripMenuItem.Name = "getCodeToolStripMenuItem";
            this.getCodeToolStripMenuItem.Size = new System.Drawing.Size(154, 26);
            this.getCodeToolStripMenuItem.Text = "Get Code";
            this.getCodeToolStripMenuItem.Click += new System.EventHandler(this.getCodeToolStripMenuItem_Click);
            // 
            // spriteToolStripMenuItem
            // 
            this.spriteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dumpToolStripMenuItem,
            this.exportAsBNSAXMLProjectToolStripMenuItem,
            this.toolStripSeparator1,
            this.replaceToolStripMenuItem,
            this.exportCurrentFrameToolStripMenuItem,
            this.exportStripToolStripMenuItem,
            this.exportedAnimagedGIFOfAnimationToolStripMenuItem});
            this.spriteToolStripMenuItem.Name = "spriteToolStripMenuItem";
            this.spriteToolStripMenuItem.Size = new System.Drawing.Size(60, 24);
            this.spriteToolStripMenuItem.Text = "Sprite";
            // 
            // dumpToolStripMenuItem
            // 
            this.dumpToolStripMenuItem.Name = "dumpToolStripMenuItem";
            this.dumpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.dumpToolStripMenuItem.Size = new System.Drawing.Size(327, 26);
            this.dumpToolStripMenuItem.Text = "Export as BNSA";
            this.dumpToolStripMenuItem.Click += new System.EventHandler(this.dumpToolStripMenuItem_Click);
            // 
            // exportAsBNSAXMLProjectToolStripMenuItem
            // 
            this.exportAsBNSAXMLProjectToolStripMenuItem.Name = "exportAsBNSAXMLProjectToolStripMenuItem";
            this.exportAsBNSAXMLProjectToolStripMenuItem.Size = new System.Drawing.Size(327, 26);
            this.exportAsBNSAXMLProjectToolStripMenuItem.Text = "Export as BNSA XML Project";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(324, 6);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(327, 26);
            this.replaceToolStripMenuItem.Text = "Replace";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // exportCurrentFrameToolStripMenuItem
            // 
            this.exportCurrentFrameToolStripMenuItem.Name = "exportCurrentFrameToolStripMenuItem";
            this.exportCurrentFrameToolStripMenuItem.Size = new System.Drawing.Size(327, 26);
            this.exportCurrentFrameToolStripMenuItem.Text = "Export Current Frame";
            this.exportCurrentFrameToolStripMenuItem.Click += new System.EventHandler(this.exportCurrentFrameToolStripMenuItem_Click);
            // 
            // exportStripToolStripMenuItem
            // 
            this.exportStripToolStripMenuItem.Name = "exportStripToolStripMenuItem";
            this.exportStripToolStripMenuItem.Size = new System.Drawing.Size(327, 26);
            this.exportStripToolStripMenuItem.Text = "ExportStrip";
            this.exportStripToolStripMenuItem.Click += new System.EventHandler(this.exportStripToolStripMenuItem_Click);
            // 
            // exportedAnimagedGIFOfAnimationToolStripMenuItem
            // 
            this.exportedAnimagedGIFOfAnimationToolStripMenuItem.Name = "exportedAnimagedGIFOfAnimationToolStripMenuItem";
            this.exportedAnimagedGIFOfAnimationToolStripMenuItem.Size = new System.Drawing.Size(327, 26);
            this.exportedAnimagedGIFOfAnimationToolStripMenuItem.Text = "Exported Animated GIF of animation";
            this.exportedAnimagedGIFOfAnimationToolStripMenuItem.Click += new System.EventHandler(this.exportedAnimatedGIFOfAnimationToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.theRockmanEXEZoneToolStripMenuItem,
            this.aboutToolStripMenuItem1});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(62, 24);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // theRockmanEXEZoneToolStripMenuItem
            // 
            this.theRockmanEXEZoneToolStripMenuItem.Name = "theRockmanEXEZoneToolStripMenuItem";
            this.theRockmanEXEZoneToolStripMenuItem.Size = new System.Drawing.Size(240, 26);
            this.theRockmanEXEZoneToolStripMenuItem.Text = "The Rockman EXE Zone";
            this.theRockmanEXEZoneToolStripMenuItem.Click += new System.EventHandler(this.theRockmanEXEZoneToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(240, 26);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // romSpritePointersListbox
            // 
            this.romSpritePointersListbox.Dock = System.Windows.Forms.DockStyle.Left;
            this.romSpritePointersListbox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.romSpritePointersListbox.FormattingEnabled = true;
            this.romSpritePointersListbox.ItemHeight = 18;
            this.romSpritePointersListbox.Location = new System.Drawing.Point(0, 28);
            this.romSpritePointersListbox.Margin = new System.Windows.Forms.Padding(4);
            this.romSpritePointersListbox.Name = "romSpritePointersListbox";
            this.romSpritePointersListbox.Size = new System.Drawing.Size(159, 461);
            this.romSpritePointersListbox.TabIndex = 1;
            this.romSpritePointersListbox.SelectedIndexChanged += new System.EventHandler(this.romSpritePointersListbox_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.paletteCountLabel);
            this.panel1.Controls.Add(this.subframeCountLabel);
            this.panel1.Controls.Add(this.frameCountLabel);
            this.panel1.Controls.Add(this.animationCountLabel);
            this.panel1.Controls.Add(this.drawScaleUpDown);
            this.panel1.Controls.Add(this.drawScaleLabel);
            this.panel1.Controls.Add(this.subframeIndexUpDown);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.animateCheckbox);
            this.panel1.Controls.Add(this.romOffsetCheckbox);
            this.panel1.Controls.Add(this.infoListbox);
            this.panel1.Controls.Add(this.paletteIndexUpDown);
            this.panel1.Controls.Add(this.frameIndexUpDown);
            this.panel1.Controls.Add(this.animationIndexUpDown);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(741, 28);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(244, 461);
            this.panel1.TabIndex = 2;
            // 
            // drawScaleUpDown
            // 
            this.drawScaleUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.drawScaleUpDown.Location = new System.Drawing.Point(179, 426);
            this.drawScaleUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.drawScaleUpDown.Name = "drawScaleUpDown";
            this.drawScaleUpDown.Size = new System.Drawing.Size(52, 22);
            this.drawScaleUpDown.TabIndex = 4;
            this.drawScaleUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.drawScaleUpDown.ValueChanged += new System.EventHandler(this.drawScaleUpDown_ValueChanged);
            // 
            // drawScaleLabel
            // 
            this.drawScaleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.drawScaleLabel.AutoSize = true;
            this.drawScaleLabel.BackColor = System.Drawing.Color.Transparent;
            this.drawScaleLabel.Location = new System.Drawing.Point(73, 428);
            this.drawScaleLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.drawScaleLabel.Name = "drawScaleLabel";
            this.drawScaleLabel.Size = new System.Drawing.Size(98, 17);
            this.drawScaleLabel.TabIndex = 12;
            this.drawScaleLabel.Text = "Drawing Scale";
            // 
            // subframeIndexUpDown
            // 
            this.subframeIndexUpDown.Enabled = false;
            this.subframeIndexUpDown.Location = new System.Drawing.Point(85, 73);
            this.subframeIndexUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.subframeIndexUpDown.Name = "subframeIndexUpDown";
            this.subframeIndexUpDown.Size = new System.Drawing.Size(103, 22);
            this.subframeIndexUpDown.TabIndex = 11;
            this.subframeIndexUpDown.ValueChanged += new System.EventHandler(this.numericUpDown4_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 75);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Subframe";
            // 
            // animateCheckbox
            // 
            this.animateCheckbox.AutoSize = true;
            this.animateCheckbox.Location = new System.Drawing.Point(9, 347);
            this.animateCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.animateCheckbox.Name = "animateCheckbox";
            this.animateCheckbox.Size = new System.Drawing.Size(81, 21);
            this.animateCheckbox.TabIndex = 9;
            this.animateCheckbox.Text = "Animate";
            this.animateCheckbox.UseVisualStyleBackColor = true;
            this.animateCheckbox.CheckedChanged += new System.EventHandler(this.animateCheckboxx_CheckedChanged);
            // 
            // romOffsetCheckbox
            // 
            this.romOffsetCheckbox.AutoSize = true;
            this.romOffsetCheckbox.Checked = true;
            this.romOffsetCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.romOffsetCheckbox.Location = new System.Drawing.Point(9, 318);
            this.romOffsetCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.romOffsetCheckbox.Name = "romOffsetCheckbox";
            this.romOffsetCheckbox.Size = new System.Drawing.Size(104, 21);
            this.romOffsetCheckbox.TabIndex = 8;
            this.romOffsetCheckbox.Text = "ROM Offset";
            this.romOffsetCheckbox.UseVisualStyleBackColor = true;
            this.romOffsetCheckbox.CheckedChanged += new System.EventHandler(this.romOffsetCheckbox_CheckedChanged);
            // 
            // infoListbox
            // 
            this.infoListbox.Enabled = false;
            this.infoListbox.FormattingEnabled = true;
            this.infoListbox.ItemHeight = 16;
            this.infoListbox.Location = new System.Drawing.Point(9, 192);
            this.infoListbox.Margin = new System.Windows.Forms.Padding(4);
            this.infoListbox.Name = "infoListbox";
            this.infoListbox.Size = new System.Drawing.Size(223, 116);
            this.infoListbox.TabIndex = 7;
            // 
            // paletteIndexUpDown
            // 
            this.paletteIndexUpDown.Location = new System.Drawing.Point(85, 103);
            this.paletteIndexUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.paletteIndexUpDown.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.paletteIndexUpDown.Name = "paletteIndexUpDown";
            this.paletteIndexUpDown.Size = new System.Drawing.Size(103, 22);
            this.paletteIndexUpDown.TabIndex = 6;
            this.paletteIndexUpDown.ValueChanged += new System.EventHandler(this.paletteUpDown_ValueChanged);
            // 
            // frameIndexUpDown
            // 
            this.frameIndexUpDown.Location = new System.Drawing.Point(85, 43);
            this.frameIndexUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.frameIndexUpDown.Name = "frameIndexUpDown";
            this.frameIndexUpDown.Size = new System.Drawing.Size(103, 22);
            this.frameIndexUpDown.TabIndex = 5;
            this.frameIndexUpDown.ValueChanged += new System.EventHandler(this.frameIndexUpDown_Changed);
            // 
            // animationIndexUpDown
            // 
            this.animationIndexUpDown.Location = new System.Drawing.Point(85, 11);
            this.animationIndexUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.animationIndexUpDown.Name = "animationIndexUpDown";
            this.animationIndexUpDown.Size = new System.Drawing.Size(103, 22);
            this.animationIndexUpDown.TabIndex = 4;
            this.animationIndexUpDown.ValueChanged += new System.EventHandler(this.animationIndexUpDown_Changed);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(17, 133);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(171, 10);
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 105);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Palette";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 45);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Frame";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Animation";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(159, 28);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(582, 461);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // spriteanimator
            // 
            this.spriteanimator.WorkerSupportsCancellation = true;
            this.spriteanimator.DoWork += new System.ComponentModel.DoWorkEventHandler(this.spriteanimator_DoWork);
            // 
            // animationCountLabel
            // 
            this.animationCountLabel.AutoSize = true;
            this.animationCountLabel.Location = new System.Drawing.Point(191, 16);
            this.animationCountLabel.Name = "animationCountLabel";
            this.animationCountLabel.Size = new System.Drawing.Size(40, 17);
            this.animationCountLabel.TabIndex = 13;
            this.animationCountLabel.Text = "of 10";
            // 
            // frameCountLabel
            // 
            this.frameCountLabel.AutoSize = true;
            this.frameCountLabel.Location = new System.Drawing.Point(191, 45);
            this.frameCountLabel.Name = "frameCountLabel";
            this.frameCountLabel.Size = new System.Drawing.Size(40, 17);
            this.frameCountLabel.TabIndex = 14;
            this.frameCountLabel.Text = "of 10";
            // 
            // subframeCountLabel
            // 
            this.subframeCountLabel.AutoSize = true;
            this.subframeCountLabel.Location = new System.Drawing.Point(191, 75);
            this.subframeCountLabel.Name = "subframeCountLabel";
            this.subframeCountLabel.Size = new System.Drawing.Size(40, 17);
            this.subframeCountLabel.TabIndex = 15;
            this.subframeCountLabel.Text = "of 10";
            // 
            // paletteCountLabel
            // 
            this.paletteCountLabel.AutoSize = true;
            this.paletteCountLabel.Location = new System.Drawing.Point(191, 105);
            this.paletteCountLabel.Name = "paletteCountLabel";
            this.paletteCountLabel.Size = new System.Drawing.Size(40, 17);
            this.paletteCountLabel.TabIndex = 16;
            this.paletteCountLabel.Text = "of 10";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 489);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.romSpritePointersListbox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Megaman BattleNetwork Sprite List Viewer ~ By Greiga Master";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.drawScaleUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.subframeIndexUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteIndexUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameIndexUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.animationIndexUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spriteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dumpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown paletteIndexUpDown;
        private System.Windows.Forms.NumericUpDown frameIndexUpDown;
        private System.Windows.Forms.NumericUpDown animationIndexUpDown;
        private System.Windows.Forms.ToolStripMenuItem exportCurrentFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchListToolStripMenuItem;
        private System.Windows.Forms.ListBox infoListbox;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem theRockmanEXEZoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem nextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.CheckBox romOffsetCheckbox;
        public System.Windows.Forms.ListBox romSpritePointersListbox;
        private System.Windows.Forms.CheckBox animateCheckbox;
        private System.ComponentModel.BackgroundWorker spriteanimator;
        private System.Windows.Forms.ToolStripMenuItem repointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportStripToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown subframeIndexUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown drawScaleUpDown;
        private System.Windows.Forms.Label drawScaleLabel;
        private System.Windows.Forms.ToolStripMenuItem openBNSAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportedAnimagedGIFOfAnimationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAsBNSAXMLProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Label paletteCountLabel;
        private System.Windows.Forms.Label subframeCountLabel;
        private System.Windows.Forms.Label frameCountLabel;
        private System.Windows.Forms.Label animationCountLabel;
    }
}

