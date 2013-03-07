using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ActionGame.MenuForms
{
    public partial class MainMenu : Form
    {
        readonly ActionGame game;
        public MainMenu(ActionGame game)
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
            this.game = game;
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
                resolutionCmb.Text = "1024x768";
            }
        }

        public GameSettings Settings
        {
            get
            {
                Match match = Regex.Match(resolutionCmb.Text, @"^(\d{3,4})x(\d{3,4})$");
                Size resolution = new  Size(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                return new GameSettings
                    {
                        Fullscreen = fullScreenChb.Checked,
                        ScreenSize = resolution,
                        TownQuarterCount = (int)quartersCountNB.Value,
                        HealBoxCount = (int)healingBoxesNB.Value,
                        AmmoBoxCount = (int)ammoBoxesNB.Value,
                        MouseXSensitivity = mouseXSensTB.Value,
                        MouseYSensitivity = mouseYSensTB.Value,
                        MouseIgnoresWindow = mouseIgnoreWindowCB.Checked,
                        MouseXInvert = mouseInvertXCB.Checked,
                        MouseYInvert = mouseInvertYCB.Checked,
                        GunSetFilename = gunSetFileTB.Text,
                        OpponentCount = (int)opponentsCountNB.Value
                    };
            }
        }
    }
}
