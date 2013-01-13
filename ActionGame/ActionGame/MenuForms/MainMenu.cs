using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ActionGame.MenuForms
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
        }

        private void playBtn_Click(object sender, EventArgs e)
        {
            resolutionCmb_TextChanged(null, null);
            DialogResult = DialogResult.OK;
        }

        private void resolutionCmb_TextChanged(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(resolutionCmb.Text, @"^\d{3,4}x\d{3,4}$"))
            {
                MessageBox.Show(
                    String.Format("Resolution in video settings contains incorrect value. You've entered \"{0}\" Correct form is for example 800x600.", resolutionCmb.Text),
                    "Bad resolution", MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1);
                resolutionCmb.Text = "640x480";
            }
        }


        public bool FullScreen { get { return fullScreenChb.Checked; } }
        public Size Resolution
        {
            get
            { 
                Match match = Regex.Match(resolutionCmb.Text, @"^(\d{3,4})x(\d{3,4})$");
                return new Size(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            }
        }
    }
}
