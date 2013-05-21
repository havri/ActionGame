namespace ActionGame.MenuForms
{
    partial class GameOver
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.townMapPB = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.winnerLbl = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.restartBTN = new System.Windows.Forms.Button();
            this.exitBtn = new System.Windows.Forms.Button();
            this.disableTime = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.townMapPB)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(796, 658);
            this.panel1.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.townMapPB);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 62);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(796, 596);
            this.panel4.TabIndex = 2;
            // 
            // townMapPB
            // 
            this.townMapPB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.townMapPB.Location = new System.Drawing.Point(0, 0);
            this.townMapPB.Name = "townMapPB";
            this.townMapPB.Size = new System.Drawing.Size(796, 596);
            this.townMapPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.townMapPB.TabIndex = 0;
            this.townMapPB.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.winnerLbl);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(796, 62);
            this.panel3.TabIndex = 1;
            // 
            // winnerLbl
            // 
            this.winnerLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winnerLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.winnerLbl.Location = new System.Drawing.Point(0, 0);
            this.winnerLbl.Name = "winnerLbl";
            this.winnerLbl.Size = new System.Drawing.Size(796, 62);
            this.winnerLbl.TabIndex = 0;
            this.winnerLbl.Text = "You won";
            this.winnerLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitContainer1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 578);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(796, 80);
            this.panel2.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.restartBTN);
            this.splitContainer1.Panel1MinSize = 50;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.exitBtn);
            this.splitContainer1.Panel2MinSize = 50;
            this.splitContainer1.Size = new System.Drawing.Size(796, 80);
            this.splitContainer1.SplitterDistance = 398;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 2;
            // 
            // restartBTN
            // 
            this.restartBTN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.restartBTN.Enabled = false;
            this.restartBTN.Location = new System.Drawing.Point(0, 0);
            this.restartBTN.Name = "restartBTN";
            this.restartBTN.Size = new System.Drawing.Size(398, 80);
            this.restartBTN.TabIndex = 1;
            this.restartBTN.Text = "Play again";
            this.restartBTN.UseVisualStyleBackColor = true;
            this.restartBTN.Click += new System.EventHandler(this.restartBTN_Click);
            // 
            // exitBtn
            // 
            this.exitBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.exitBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exitBtn.Enabled = false;
            this.exitBtn.Location = new System.Drawing.Point(0, 0);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(397, 80);
            this.exitBtn.TabIndex = 0;
            this.exitBtn.Text = "Exit the game";
            this.exitBtn.UseVisualStyleBackColor = true;
            this.exitBtn.Click += new System.EventHandler(this.exitBtn_Click);
            // 
            // disableTime
            // 
            this.disableTime.Enabled = true;
            this.disableTime.Interval = 2000;
            this.disableTime.Tick += new System.EventHandler(this.disableTime_Tick);
            // 
            // GameOver
            // 
            this.AcceptButton = this.restartBTN;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.exitBtn;
            this.ClientSize = new System.Drawing.Size(796, 658);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "GameOver";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Game over";
            this.panel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.townMapPB)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label winnerLbl;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button restartBTN;
        private System.Windows.Forms.Button exitBtn;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.PictureBox townMapPB;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Timer disableTime;
    }
}