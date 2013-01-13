using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ActionGame.MenuForms
{
    public partial class Loading : Form
    {
        public Loading()
        {
            InitializeComponent();
            progressBar.Value = 0;
            percentageLabel.Text = "0%";
            progressLabel.Text = String.Empty;
        }

        public void SetValue(int p)
        {
            progressBar.Value = p;
            percentageLabel.Text = String.Format("{0}%",p);
            Refresh();
            Application.DoEvents();
        }

        public void SetLabel(string l)
        {
            progressLabel.Text = l;
            Refresh();
            Application.DoEvents();
        }
    }
}
