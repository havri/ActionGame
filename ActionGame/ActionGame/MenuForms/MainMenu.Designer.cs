namespace ActionGame.MenuForms
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
            this.videoSettingsPage = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.playBtn = new System.Windows.Forms.Button();
            this.fullScreenChb = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.resolutionCmb = new System.Windows.Forms.ComboBox();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.videoSettingsPage.SuspendLayout();
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
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.gameSettingsPage);
            this.tabControl1.Controls.Add(this.videoSettingsPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(584, 437);
            this.tabControl1.TabIndex = 1;
            // 
            // gameSettingsPage
            // 
            this.gameSettingsPage.Location = new System.Drawing.Point(4, 22);
            this.gameSettingsPage.Name = "gameSettingsPage";
            this.gameSettingsPage.Padding = new System.Windows.Forms.Padding(3);
            this.gameSettingsPage.Size = new System.Drawing.Size(576, 411);
            this.gameSettingsPage.TabIndex = 0;
            this.gameSettingsPage.Text = "Game";
            this.gameSettingsPage.UseVisualStyleBackColor = true;
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
            this.videoSettingsPage.Size = new System.Drawing.Size(576, 411);
            this.videoSettingsPage.TabIndex = 1;
            this.videoSettingsPage.Text = "Video";
            this.videoSettingsPage.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.playBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 371);
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Screen mode";
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
            // resolutionCmb
            // 
            this.resolutionCmb.FormattingEnabled = true;
            this.resolutionCmb.Items.AddRange(new object[] {
            "640x480",
            "800x450",
            "800x500",
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
            this.resolutionCmb.TabIndex = 3;
            this.resolutionCmb.TextChanged += new System.EventHandler(this.resolutionCmb_TextChanged);
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainMenu";
            this.Text = "MainMenu";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.videoSettingsPage.ResumeLayout(false);
            this.videoSettingsPage.PerformLayout();
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
    }
}