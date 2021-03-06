﻿namespace ActionGame.MenuForms
{
    partial class MainMenu
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
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.gameSettingsPage = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.healingBoxesNB = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.ammoBoxesNB = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.quartersCountNB = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.videoSettingsPage = new System.Windows.Forms.TabPage();
            this.resolutionCmb = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.fullScreenChb = new System.Windows.Forms.CheckBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.keyselectorsFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label18 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mouseInvertXCB = new System.Windows.Forms.CheckBox();
            this.mouseInvertYCB = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.mouseYSensTB = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.mouseXSensTB = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.playBtn = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.gameSettingsPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.healingBoxesNB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ammoBoxesNB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.quartersCountNB)).BeginInit();
            this.videoSettingsPage.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mouseYSensTB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mouseXSensTB)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(584, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.gameSettingsPage);
            this.tabControl1.Controls.Add(this.videoSettingsPage);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(584, 592);
            this.tabControl1.TabIndex = 1;
            // 
            // gameSettingsPage
            // 
            this.gameSettingsPage.Controls.Add(this.groupBox1);
            this.gameSettingsPage.Location = new System.Drawing.Point(4, 22);
            this.gameSettingsPage.Name = "gameSettingsPage";
            this.gameSettingsPage.Padding = new System.Windows.Forms.Padding(3);
            this.gameSettingsPage.Size = new System.Drawing.Size(576, 566);
            this.gameSettingsPage.TabIndex = 0;
            this.gameSettingsPage.Text = "Game";
            this.gameSettingsPage.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.healingBoxesNB);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.ammoBoxesNB);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.quartersCountNB);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(570, 96);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Town";
            // 
            // healingBoxesNB
            // 
            this.healingBoxesNB.Location = new System.Drawing.Point(140, 40);
            this.healingBoxesNB.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.healingBoxesNB.Name = "healingBoxesNB";
            this.healingBoxesNB.Size = new System.Drawing.Size(40, 20);
            this.healingBoxesNB.TabIndex = 1;
            this.healingBoxesNB.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(121, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Ammo boxes per quarter";
            // 
            // ammoBoxesNB
            // 
            this.ammoBoxesNB.Location = new System.Drawing.Point(140, 65);
            this.ammoBoxesNB.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.ammoBoxesNB.Name = "ammoBoxesNB";
            this.ammoBoxesNB.Size = new System.Drawing.Size(40, 20);
            this.ammoBoxesNB.TabIndex = 2;
            this.ammoBoxesNB.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Number of quarters";
            // 
            // quartersCountNB
            // 
            this.quartersCountNB.Location = new System.Drawing.Point(140, 14);
            this.quartersCountNB.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.quartersCountNB.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.quartersCountNB.Name = "quartersCountNB";
            this.quartersCountNB.Size = new System.Drawing.Size(40, 20);
            this.quartersCountNB.TabIndex = 0;
            this.quartersCountNB.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Healing boxes per quarter";
            // 
            // videoSettingsPage
            // 
            this.videoSettingsPage.Controls.Add(this.resolutionCmb);
            this.videoSettingsPage.Controls.Add(this.label2);
            this.videoSettingsPage.Controls.Add(this.label1);
            this.videoSettingsPage.Controls.Add(this.fullScreenChb);
            this.videoSettingsPage.Location = new System.Drawing.Point(4, 22);
            this.videoSettingsPage.Name = "videoSettingsPage";
            this.videoSettingsPage.Padding = new System.Windows.Forms.Padding(3);
            this.videoSettingsPage.Size = new System.Drawing.Size(576, 566);
            this.videoSettingsPage.TabIndex = 1;
            this.videoSettingsPage.Text = "Video";
            this.videoSettingsPage.UseVisualStyleBackColor = true;
            // 
            // resolutionCmb
            // 
            this.resolutionCmb.FormattingEnabled = true;
            this.resolutionCmb.Items.AddRange(new object[] {
            "800x600",
            "1024x576",
            "1024x640",
            "1024x768",
            "1280x720",
            "1280x800",
            "1280x960",
            "1366x768",
            "1366x854",
            "1366x1024",
            "1440x810",
            "1440x900",
            "1440x1080",
            "1680x945",
            "1680x1050",
            "1680x1260",
            "1920x1080",
            "1920x1200",
            "1920x1440"});
            this.resolutionCmb.Location = new System.Drawing.Point(84, 29);
            this.resolutionCmb.Name = "resolutionCmb";
            this.resolutionCmb.Size = new System.Drawing.Size(121, 21);
            this.resolutionCmb.TabIndex = 1;
            this.resolutionCmb.Text = "1280x800";
            this.resolutionCmb.TextChanged += new System.EventHandler(this.resolutionCmb_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Resolution";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Screen mode";
            // 
            // fullScreenChb
            // 
            this.fullScreenChb.AutoSize = true;
            this.fullScreenChb.Location = new System.Drawing.Point(84, 6);
            this.fullScreenChb.Name = "fullScreenChb";
            this.fullScreenChb.Size = new System.Drawing.Size(77, 17);
            this.fullScreenChb.TabIndex = 0;
            this.fullScreenChb.Text = "Full screen";
            this.fullScreenChb.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(576, 566);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Controls";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.keyselectorsFlowPanel);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 199);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(570, 364);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Keyboard";
            // 
            // keyselectorsFlowPanel
            // 
            this.keyselectorsFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.keyselectorsFlowPanel.Location = new System.Drawing.Point(6, 32);
            this.keyselectorsFlowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.keyselectorsFlowPanel.Name = "keyselectorsFlowPanel";
            this.keyselectorsFlowPanel.Size = new System.Drawing.Size(558, 243);
            this.keyselectorsFlowPanel.TabIndex = 0;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Enabled = false;
            this.label18.Location = new System.Drawing.Point(6, 16);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(175, 13);
            this.label18.TabIndex = 26;
            this.label18.Text = "Click inside the textbox to change...";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mouseInvertXCB);
            this.groupBox2.Controls.Add(this.mouseInvertYCB);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.mouseYSensTB);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.mouseXSensTB);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(570, 196);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mouse";
            // 
            // mouseInvertXCB
            // 
            this.mouseInvertXCB.AutoSize = true;
            this.mouseInvertXCB.Location = new System.Drawing.Point(114, 121);
            this.mouseInvertXCB.Name = "mouseInvertXCB";
            this.mouseInvertXCB.Size = new System.Drawing.Size(94, 17);
            this.mouseInvertXCB.TabIndex = 2;
            this.mouseInvertXCB.Text = "Horizontal axis";
            this.mouseInvertXCB.UseVisualStyleBackColor = true;
            // 
            // mouseInvertYCB
            // 
            this.mouseInvertYCB.AutoSize = true;
            this.mouseInvertYCB.Location = new System.Drawing.Point(114, 144);
            this.mouseInvertYCB.Name = "mouseInvertYCB";
            this.mouseInvertYCB.Size = new System.Drawing.Size(82, 17);
            this.mouseInvertYCB.TabIndex = 3;
            this.mouseInvertYCB.Text = "Vertikal axix";
            this.mouseInvertYCB.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(74, 122);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Invert";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 73);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Vertical sensitivity";
            // 
            // mouseYSensTB
            // 
            this.mouseYSensTB.LargeChange = 20;
            this.mouseYSensTB.Location = new System.Drawing.Point(114, 70);
            this.mouseYSensTB.Maximum = 110;
            this.mouseYSensTB.Minimum = 10;
            this.mouseYSensTB.Name = "mouseYSensTB";
            this.mouseYSensTB.Size = new System.Drawing.Size(145, 45);
            this.mouseYSensTB.SmallChange = 5;
            this.mouseYSensTB.TabIndex = 1;
            this.mouseYSensTB.Value = 60;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Horizontal sensitivity";
            // 
            // mouseXSensTB
            // 
            this.mouseXSensTB.LargeChange = 20;
            this.mouseXSensTB.Location = new System.Drawing.Point(114, 19);
            this.mouseXSensTB.Maximum = 110;
            this.mouseXSensTB.Minimum = 10;
            this.mouseXSensTB.Name = "mouseXSensTB";
            this.mouseXSensTB.Size = new System.Drawing.Size(145, 45);
            this.mouseXSensTB.SmallChange = 5;
            this.mouseXSensTB.TabIndex = 0;
            this.mouseXSensTB.Value = 60;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.playBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 526);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(584, 90);
            this.panel1.TabIndex = 2;
            // 
            // playBtn
            // 
            this.playBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.playBtn.Location = new System.Drawing.Point(423, 0);
            this.playBtn.Name = "playBtn";
            this.playBtn.Size = new System.Drawing.Size(161, 90);
            this.playBtn.TabIndex = 0;
            this.playBtn.Text = "Play!";
            this.playBtn.UseVisualStyleBackColor = true;
            this.playBtn.Click += new System.EventHandler(this.playBtn_Click);
            // 
            // MainMenu
            // 
            this.AcceptButton = this.playBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 616);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainMenu";
            this.ShowIcon = false;
            this.Text = "Action game";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.gameSettingsPage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.healingBoxesNB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ammoBoxesNB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.quartersCountNB)).EndInit();
            this.videoSettingsPage.ResumeLayout(false);
            this.videoSettingsPage.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mouseYSensTB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mouseXSensTB)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage gameSettingsPage;
        private System.Windows.Forms.TabPage videoSettingsPage;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button playBtn;
        private System.Windows.Forms.ComboBox resolutionCmb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox fullScreenChb;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown quartersCountNB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown healingBoxesNB;
        private System.Windows.Forms.NumericUpDown ammoBoxesNB;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar mouseXSensTB;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TrackBar mouseYSensTB;
        private System.Windows.Forms.CheckBox mouseInvertXCB;
        private System.Windows.Forms.CheckBox mouseInvertYCB;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.FlowLayoutPanel keyselectorsFlowPanel;
    }
}