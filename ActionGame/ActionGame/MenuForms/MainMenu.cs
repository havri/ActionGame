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

        KeySelector ForwardSelector { get; set; }
        KeySelector BackwardSelector { get; set; }
        KeySelector StepLeftSelector { get; set; }
        KeySelector StepRightSelector { get; set; }
        KeySelector TurnLeftSelector { get; set; }
        KeySelector TurnRightSelector { get; set; }
        KeySelector TurnUpSelector { get; set; }
        KeySelector TurnDownSelector { get; set; }
        KeySelector RunWalkSwitchSelector { get; set; }
        KeySelector ShowQuarterMapSelector { get; set; }
        KeySelector ShowTownMapSelector { get; set; }
        KeySelector CameraSwitchSelector { get; set; }

        public MainMenu(ActionGame game, bool videoEnabled)
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
            this.game = game;
            if (!videoEnabled)
            {
                //tabControl1.TabPages.RemoveAt(1);
                fullScreenChb.Enabled = false;
                resolutionCmb.Enabled = false;
                fullScreenChb.Checked = game.Settings.Fullscreen;
                resolutionCmb.Text = String.Format("{0}x{1}", game.Settings.ScreenSize.Width, game.Settings.ScreenSize.Height);
            }

            ForwardSelector = new KeySelector("Move forward", Microsoft.Xna.Framework.Input.Keys.W);
            BackwardSelector = new KeySelector("Move backward", Microsoft.Xna.Framework.Input.Keys.S);
            StepLeftSelector = new KeySelector("Left step", Microsoft.Xna.Framework.Input.Keys.A);
            StepRightSelector = new KeySelector("Right step", Microsoft.Xna.Framework.Input.Keys.D);
            TurnLeftSelector = new KeySelector("Turn left", Microsoft.Xna.Framework.Input.Keys.Left);
            TurnRightSelector = new KeySelector("Turn right", Microsoft.Xna.Framework.Input.Keys.Right);
            TurnUpSelector = new KeySelector("Turn up", Microsoft.Xna.Framework.Input.Keys.Up);
            TurnDownSelector = new KeySelector("Turn down", Microsoft.Xna.Framework.Input.Keys.Down);
            RunWalkSwitchSelector = new KeySelector("Run / Walk", Microsoft.Xna.Framework.Input.Keys.CapsLock);
            ShowQuarterMapSelector = new KeySelector("Show quarter map", Microsoft.Xna.Framework.Input.Keys.M);
            ShowTownMapSelector = new KeySelector("Show town map", Microsoft.Xna.Framework.Input.Keys.N);
            CameraSwitchSelector = new KeySelector("Change camera mode", Microsoft.Xna.Framework.Input.Keys.C);
            keyselectorsFlowPanel.Controls.Add(ForwardSelector);
            keyselectorsFlowPanel.Controls.Add(BackwardSelector);
            keyselectorsFlowPanel.Controls.Add(StepLeftSelector);
            keyselectorsFlowPanel.Controls.Add(StepRightSelector);
            keyselectorsFlowPanel.Controls.Add(TurnLeftSelector);
            keyselectorsFlowPanel.Controls.Add(TurnRightSelector);
            keyselectorsFlowPanel.Controls.Add(TurnUpSelector);
            keyselectorsFlowPanel.Controls.Add(TurnDownSelector);
            keyselectorsFlowPanel.Controls.Add(RunWalkSwitchSelector);
            keyselectorsFlowPanel.Controls.Add(ShowQuarterMapSelector);
            keyselectorsFlowPanel.Controls.Add(ShowTownMapSelector);
            keyselectorsFlowPanel.Controls.Add(CameraSwitchSelector);
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
                        MouseIgnoresWindow = true,
                        MouseXInvert = mouseInvertXCB.Checked,
                        MouseYInvert = mouseInvertYCB.Checked,
                        GunSetFilename = "DefaultGunSet",

                        Forward = ForwardSelector.Value,
                        Backward = BackwardSelector.Value,
                        StepLeft = StepLeftSelector.Value,
                        StepRight = StepRightSelector.Value,
                        TurnLeft = TurnLeftSelector.Value,
                        TurnRight = TurnRightSelector.Value,
                        TurnUp = TurnUpSelector.Value,
                        TurnDown = TurnDownSelector.Value,
                        RunWalkSwitch = RunWalkSwitchSelector.Value,
                        ShowQuarterMap = ShowQuarterMapSelector.Value,
                        ShowTownMap = ShowTownMapSelector.Value,
                        CameraSwitch = CameraSwitchSelector.Value
                    };
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ActionGame - 3D action game in a bizzare city\n\nThis is a Bachelor Thesis implementation part.\n\nCharles University in Prague\nFaculty of Mathematics and Physics\n\n2013 Štěpán Havránek", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
