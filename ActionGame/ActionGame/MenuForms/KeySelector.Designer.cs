namespace ActionGame.MenuForms
{
    partial class KeySelector
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
            this.label = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.Location = new System.Drawing.Point(3, 6);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(120, 13);
            this.label.TabIndex = 0;
            this.label.Text = "label1";
            this.label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(129, 3);
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.Size = new System.Drawing.Size(100, 20);
            this.textBox.TabIndex = 1;
            this.textBox.Click += new System.EventHandler(this.CatchKeyTextBoxEntered);
            this.textBox.Enter += new System.EventHandler(this.CatchKeyTextBoxEntered);
            // 
            // KeySelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.label);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "KeySelector";
            this.Size = new System.Drawing.Size(235, 26);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.TextBox textBox;
    }
}
