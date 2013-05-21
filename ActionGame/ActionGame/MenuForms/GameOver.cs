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
    public partial class GameOver : Form
    {
        public GameOver(bool playerWon, Image townMap)
        {
            InitializeComponent();
            winnerLbl.Text = "Game over. " + (playerWon ? "You are the winner!" : "You have lost.");
            winnerLbl.ForeColor = (playerWon ? Color.Green : Color.Red);
            townMapPB.Image = townMap;
        }

        private void restartBTN_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void disableTime_Tick(object sender, EventArgs e)
        {
            restartBTN.Enabled = true;
            exitBtn.Enabled = true;
            disableTime.Enabled = false;
        }

        
    }
}
